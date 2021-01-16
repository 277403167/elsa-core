using System;
using System.Collections.Generic;
using System.Linq;
using Elsa.Services;
using Elsa.Services.Models;

namespace Elsa.Builders
{
    public class ActivityBuilder : IActivityBuilder
    {
        public ActivityBuilder(
            Type activityType,
            ICompositeActivityBuilder workflowBuilder,
            IDictionary<string, IActivityPropertyValueProvider>? propertyValueProviders)
        {
            ActivityType = activityType;
            WorkflowBuilder = workflowBuilder;
            PropertyValueProviders = propertyValueProviders;
        }
        
        protected ActivityBuilder()
        {
        }

        public Type ActivityType { get; protected set; } = default!;
        public ICompositeActivityBuilder WorkflowBuilder { get; set; } = default!;
        public string ActivityId { get; set; } = default!;
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public bool PersistWorkflowEnabled { get; set; }
        public bool LoadWorkflowContextEnabled { get; set; }
        public bool SaveWorkflowContextEnabled { get; set; }
        public bool PersistOutputEnabled { get; set; }
        public string? Source { get; set; }

        public IDictionary<string, IActivityPropertyValueProvider>? PropertyValueProviders { get; protected set; }

        public IActivityBuilder Add<T>(
            Action<ISetupActivity<T>>? setup = default, int lineNumber = default, string? sourceFile = default)
            where T : class, IActivity =>
            WorkflowBuilder.Add(setup, null, lineNumber, sourceFile);

        public IOutcomeBuilder When(string outcome) => new OutcomeBuilder(WorkflowBuilder, this, outcome);

        public virtual IActivityBuilder Then<T>(
            Action<ISetupActivity<T>>? setup = null,
            Action<IActivityBuilder>? branch = null, int lineNumber = default, string? sourceFile = default)
            where T : class, IActivity =>
            When(OutcomeNames.Done).Then(setup, branch, lineNumber, sourceFile);

        public virtual IActivityBuilder Then<T>(Action<IActivityBuilder>? branch = null, int lineNumber = default, string? sourceFile = default)
            where T : class, IActivity =>
            When(OutcomeNames.Done).Then<T>(branch, lineNumber, sourceFile);

        public virtual IActivityBuilder Then(IActivityBuilder targetActivity)
        {
            WorkflowBuilder.Connect(this, targetActivity);
            return this;
        }

        public virtual IConnectionBuilder Then(string activityName) =>
            WorkflowBuilder.Connect(
                () => this,
                () => WorkflowBuilder.Activities.First(x => x.Name == activityName));

        public IActivityBuilder WithId(string? value)
        {
            ActivityId = value!;
            return this;
        }

        public IActivityBuilder WithName(string? value)
        {
            Name = value;
            return this;
        }
        
        public IActivityBuilder WithDisplayName(string? value)
        {
            DisplayName = value;
            return this;
        }
        
        public IActivityBuilder WithDescription(string? value)
        {
            Description = value;
            return this;
        }
        
        public IActivityBuilder LoadWorkflowContext(bool value)
        {
            LoadWorkflowContextEnabled = value;
            return this;
        }
        
        public IActivityBuilder SaveWorkflowContext(bool value)
        {
            SaveWorkflowContextEnabled = value;
            return this;
        }
        
        public IActivityBuilder PersistOutput(bool value)
        {
            PersistOutputEnabled = value;
            return this;
        }
        
        public IActivityBuilder PersistWorkflow(bool value)
        {
            PersistWorkflowEnabled = value;
            return this;
        }

        public IWorkflowBlueprint Build() => ((IWorkflowBuilder)WorkflowBuilder).BuildBlueprint();
    }
}
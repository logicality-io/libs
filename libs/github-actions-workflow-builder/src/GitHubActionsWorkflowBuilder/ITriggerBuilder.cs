namespace Logicality.GitHubActionsWorkflowBuilder;

public interface ITriggerBuilder
{
    string EventName { get; }

    WorkflowBuilder WorkflowBuilder { get; }
}
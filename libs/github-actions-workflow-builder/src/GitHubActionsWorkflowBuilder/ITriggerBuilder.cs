namespace Logicality.GithubActionsWorkflowBuilder;

public interface ITriggerBuilder
{
    string EventName { get; }

    WorkflowBuilder WorkflowBuilder { get; }
}
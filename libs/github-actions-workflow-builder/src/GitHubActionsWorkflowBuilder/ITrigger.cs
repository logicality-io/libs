namespace Logicality.GitHubActionsWorkflowBuilder;

public interface ITrigger
{
    string EventName { get; }

    WorkflowBuilder WorkflowBuilder { get; }
}
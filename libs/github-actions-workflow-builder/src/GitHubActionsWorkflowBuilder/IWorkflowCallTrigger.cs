namespace Logicality.GitHubActionsWorkflowBuilder;

public interface IWorkflowCallTrigger : ITrigger
{
    IWorkflowCallTrigger Input(
        string           id,
        string           description,
        string           @default,
        bool             required,
        WorkflowCallType workflowCallType);

    IWorkflowCallTrigger Output(
        string id,
        string description,
        string value);
}
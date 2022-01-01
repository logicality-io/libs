namespace Logicality.GitHubActionsWorkflowBuilder;

public interface IWorkflowCallTrigger : ITrigger
{
    IWorkflowCallTrigger Inputs(
        string           id,
        string           description,
        string           @default,
        bool             required,
        WorkflowCallType workflowCallType);

    IWorkflowCallTrigger Outputs(
        string id,
        string description,
        string value);

    IWorkflowCallTrigger Secrets(
        string secretId,
        string description,
        bool   required);
}
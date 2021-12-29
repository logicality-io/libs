namespace Logicality.GitHubActionsWorkflowBuilder;

public interface IStepBuilder
{
    IJobBuilder Job { get; }

    IStepBuilder If(string condition);

    IStepBuilder Uses(string uses);

    IStepBuilder With(string name, string value);

    IStepBuilder Run(string run);

    IStepBuilder Shell(string run);
}
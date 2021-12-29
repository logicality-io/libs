namespace Logicality.GitHubActionsWorkflowBuilder;

public interface IJobBuilder
{
    IJobBuilder RunsOn(string runsOn);

    IJobBuilder WithEnvironment(IDictionary<string, string> environment);

    IStepBuilder AddStep(string name);
}
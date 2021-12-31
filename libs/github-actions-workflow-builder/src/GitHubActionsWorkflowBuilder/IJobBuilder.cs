namespace Logicality.GitHubActionsWorkflowBuilder;

public interface IJobBuilder
{
    IJobBuilder RunsOn(string runsOn);

    IJobBuilder WithEnvironment(IDictionary<string, string> environment);

    IJobBuilder Permissions(
        Permission actions = Permission.None,
        Permission checks = Permission.None,
        Permission contents = Permission.None,
        Permission deployments = Permission.None,
        Permission idToken = Permission.None,
        Permission issues = Permission.None,
        Permission discussions = Permission.None,
        Permission packages = Permission.None,
        Permission pages = Permission.None,
        Permission pullRequests = Permission.None,
        Permission repositoryProjects = Permission.None,
        Permission securityEvents = Permission.None,
        Permission statuses = Permission.None);

    IJobBuilder Concurrency(string group, bool cancelInProgress = false);

    IStepBuilder AddStep(string name);
}

public enum Permission
{
    Read,
    Write,
    None
}
using Logicality.GithubActionsWorkflowBuilder;

namespace Logicality.GitHubActionsWorkflowBuilder;

public static class JobExtensions
{
    public static IJobBuilder Checkout(this IJobBuilder job, int fetchDepth = 0)
    {
        job.AddStep()
            .Name("Checkout")
            .Uses("actions/checkout@v2")
            .With("fetch-depth", fetchDepth.ToString());
        return job;
    }

    public static IJobBuilder LogIntoGitHubContainerRegistry(this IJobBuilder job)
    {
        job.AddStep()
            .Name("Log into GitHub Container Registry")
            .Run("echo \"${{secrets.GITHUB_TOKEN}}\" | docker login ghcr.io -u ${{ github.actor }} --password-stdin");
        return job;
    }

    public static IJobBuilder UploadArtifacts(
        this IJobBuilder job,
        string           name = "artifacts",
        string           path = "artifacts")
    {
        job.AddStep()
            .Name("Upload artifacts")
            .Uses("actions/upload-artifact@v2")
            .With("name", name)
            .With("path", path);
        return job;
    }

    public static IJobBuilder PrintEnvironment(this IJobBuilder job)
    {
        job.AddStep()
            .Name("Print Environment")
            .Run("printenv")
            .ShellBash();
        return job;
    }
}


public static class IStepBuilderExtensions
{
    public static IStepBuilder ShellBash(this IStepBuilder step)
    {
        step.Shell("bash");
        return step;
    }

    public static IStepBuilder ShellPowerShell(this IStepBuilder step)
    {
        step.Shell("pwsh");
        return step;
    }
}
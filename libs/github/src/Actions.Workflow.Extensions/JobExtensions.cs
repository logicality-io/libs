namespace Logicality.GitHub.Actions.Workflow;

public static class JobExtensions
{
    public static Job StepActionsCheckout(
        this Job job,
        string   version    = "v3",
        int      fetchDepth = 0)
    {
        job.Step()
            .Name("Checkout")
            .Uses($"actions/checkout@{version}")
            .With("fetch-depth", fetchDepth.ToString());
        return job;
    }

    public static Job StepLogIntoGitHubContainerRegistry(this Job job)
    {
        job.Step()
            .Name("Log into GitHub Container Registry")
            .Run("echo \"${{secrets.GITHUB_TOKEN}}\" | docker login ghcr.io -u ${{github.actor}} --password-stdin");
        return job;
    }

    public static Job StepActionsUploadArtifact(
        this Job job,
        string   name    = "artifacts",
        string   path    = "artifacts",
        string   version = "v2")
    {
        job.Step()
            .Name("Upload Artifacts")
            .Uses($"actions/upload-artifact@{version}")
            .With("name", name)
            .With("path", path);
        return job;
    }

    public static Job StepPrintEnvironment(this Job job)
    {
        job.Step()
            .Name("Print Env")
            .Run("printenv")
            .ShellBash();
        return job;
    }
}
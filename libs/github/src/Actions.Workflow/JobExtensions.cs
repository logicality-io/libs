namespace Logicality.GitHub.Actions.Workflow;

public static class JobExtensions
{
    public static Job CheckoutStep(this Job job, int actionVersion = 3, int fetchDepth = 0)
    {
        job.Step()
            .Name("Checkout")
            .Uses($"actions/checkout@v{actionVersion}")
            .With("fetch-depth", fetchDepth.ToString());
        return job;
    }

    public static Job LogIntoGitHubContainerRegistryStep(this Job job)
    {
        job.Step()
            .Name("Log into GitHub Container Registry")
            .Run("echo \"${{secrets.GITHUB_TOKEN}}\" | docker login ghcr.io -u ${{github.actor}} --password-stdin");
        return job;
    }

    public static Job UploadArtifacts(
        this Job job,
        string   name = "artifacts",
        string   path = "artifacts")
    {
        job.Step()
            .Name("Upload Artifacts")
            .Uses("actions/upload-artifact@v2")
            .With("name", name)
            .With("path", path);
        return job;
    }

    public static Job PrintEnvironmentStep(this Job job)
    {
        job.Step()
            .Name("Print Env")
            .Run("printenv")
            .ShellBash();
        return job;
    }
}
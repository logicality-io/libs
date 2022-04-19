namespace Logicality.GitHub.Actions.Workflow;

public static class JobExtensions
{
    public static Job ActionsCheckout(
        this Step step,
        string   version    = "v3",
        int      fetchDepth = 0)
    {
        step
            .Name("Checkout")
            .Uses($"actions/checkout@{version}")
            .With()
            .Key("fetch-depth", fetchDepth.ToString());
        return step.Job;
    }

    /// <summary>
    /// Logs into the GitHub container registry.
    /// </summary>
    /// <param name="step">The Step.</param>
    /// <returns>The associated job</returns>
    public static Job LogIntoGitHubContainerRegistry(this Step step)
    {
        step
            .Name("Log into GitHub Container Registry")
            .Run("echo \"${{secrets.GITHUB_TOKEN}}\" | docker login ghcr.io -u ${{github.actor}} --password-stdin");
        return step.Job;
    }

    /// <summary>
    /// Upload artifacts using 'actions/upload-artifacts' action.
    /// </summary>
    /// <param name="step">The step.</param>
    /// <param name="name">The name of the artifacts package.</param>
    /// <param name="path">The to the artifacts directory.</param>
    /// <param name="version"></param>
    /// <returns>The associated job.</returns>
    public static Job ActionsUploadArtifact(
        this Step step,
        string    name    = "artifacts",
        string    path    = "artifacts",
        string    version = "v3")
    {
        step.Name("Upload Artifacts")
            .Uses($"actions/upload-artifact@{version}")
            .With()
                .Key("name", name)
                .Key("path", path);
        return step.Job;
    }

    /// <summary>
    /// Dissplay the values of environment variables using 'printenv'.
    /// </summary>
    /// <param name="step"></param>
    /// <returns></returns>
    public static Job PrintEnvironment(this Step step)
    {
        step
            .Name("Print Env")
            .Run("printenv")
            .Shell(Shells.Bash);
        return step.Job;
    }
}
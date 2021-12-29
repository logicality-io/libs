using Logicality.GitHubActionsWorkflowBuilder;

var libs = new []
{
    "aspnet-core",
    "bullseye",
    "configuration",
    "github-actions-workflow-builder",
    "hosting",
    "lambda",
    "pulumi",
    "system-extensions",
    "testing"
};

var path = "../workflows";

foreach (var lib in libs)
{
    var workflow = new WorkflowBuilder($"{lib}-ci");

    var paths = new[] { $".github/workflows/{lib}-**", $"libs/{lib}**", "build/**" };

    workflow.OnPullRequest()
        .Paths(paths);

    workflow.OnPush()
        .Branches("main")
        .Paths(paths)
        .Tags($"'{lib}-**'");

    var job = workflow.AddJob("build")
        .RunsOn("ubuntu-latest")
        .WithEnvironment(new Dictionary<string, string>
        {
            { "GITHUB_TOKEN", "${{secrets.GITHUB_TOKEN}}" }
        });

    job.Checkout();

    job.LogIntoGitHubContainerRegistry();

    job.PrintEnvironment();

    job.AddStep()
        .Name("Test")
        .Run($"./build.ps1 {lib}-test")
        .ShellPowerShell();

    job.AddStep()
        .Name("Pack")
        .Run($"./build.ps1 {lib}-pack")
        .ShellPowerShell();

    job.AddStep()
        .Name("Push")
        .If("github.event_name == 'push'")
        .Run("./build.ps1 push")
        .ShellPowerShell();

    job.UploadArtifacts();

    var yaml     = workflow.Generate();
    var fileName = $"{lib}-ci.yml";
    var filePath = $"{path}/{fileName}";

    File.WriteAllText(filePath, yaml);

    Console.WriteLine($"Wrote workflow to {filePath}");
}
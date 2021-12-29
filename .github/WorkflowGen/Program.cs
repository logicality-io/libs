using Logicality.GithubActionsWorkflowBuilder;

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
        .Tags($"'{lib}-**");

    var job = workflow.AddJob("build")
        .RunsOn("ubuntu-latest")
        .WithEnvironment(new Dictionary<string, string>
        {
            { "GITHUB_TOKEN", "${{secrets.GITHUB_TOKEN}}" }
        });

    job.AddStep()
        .Name("Checkout")
        .Uses("actions/checkout@v2")
        .With("fetch-depth", "0");

    job.AddStep()
        .Name("Log into ghcr")
        .Run("echo \"${{secrets.GITHUB_TOKEN}}\" | docker login ghcr.io -u ${{ github.actor }} --password-stdin");

    job.AddStep()
        .Name("Print Environment")
        .Run("printenv")
        .Shell("bash");

    job.AddStep()
        .Name("Test")
        .Run($"./build.ps1 {lib}-test")
        .Shell("pwsh");

    job.AddStep()
        .Name("Pack")
        .Run($"./build.ps1 {lib}-pack")
        .Shell("pwsh");

    job.AddStep()
        .Name("Push")
        .If("github.event_name == 'push'")
        .Run("./build.ps1 push");

    job.AddStep()
        .Name("Upload artifacts")
        .Uses("actions/upload-artifact@v2")
        .With("name", "artifcats")
        .With("path", "artifacts");

    var yaml     = workflow.Generate();
    var fileName = $"{lib}-ci.yml";
    var filePath = $"{path}/{fileName}";

    File.WriteAllText(filePath, yaml);

    Console.WriteLine($"Wrote workflow to {filePath}");
}
using Logicality.GitHubActionsWorkflowBuilder;

void WriteWorkflow(WorkflowBuilder workflow, string fileName)
{
    var path = "../workflows";
    var yaml     = workflow.Generate();
    var filePath = $"{path}/{fileName}.yml";

    File.WriteAllText(filePath, yaml);

    Console.WriteLine($"Wrote workflow to {filePath}");
}

void GenerateWorkflowsForLibs()
{
    var libs = new[]
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

        job.AddStep("Test")
            .Run($"./build.ps1 {lib}-test")
            .ShellPowerShell();

        job.AddStep("Pack")
            .Run($"./build.ps1 {lib}-pack")
            .ShellPowerShell();

        job.AddStep("Push")
            .If("github.event_name == 'push'")
            .Run("./build.ps1 push")
            .ShellPowerShell();

        job.UploadArtifacts();

        var fileName = $"{lib}-ci";

        WriteWorkflow(workflow, fileName);
    }
}

void GenerateCodeAnalysisWorkflow()
{
    var workflow = new WorkflowBuilder("CodeQL");

    workflow.OnPush()
        .Branches("main");
    workflow.OnPullRequest()
        .Branches("main");
    workflow.OnSchedule("'39 8 * * 1'");

    var job = workflow.AddJob("analyze")
        .RunsOn("ubuntu-latest")
        .Permissions(
            actions: Permission.Read,
            contents: Permission.Read,
            securityEvents: Permission.Write);

    job.AddStep("");
}


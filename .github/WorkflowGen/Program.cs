using Logicality.GitHub.Actions.Workflow;

void WriteWorkflow(Workflow workflow, string fileName)
{
    var filePath = $"../workflows/{fileName}.yml";
    workflow.WriteYaml(filePath);
    Console.WriteLine($"Wrote workflow to {filePath}");
}

void GenerateWorkflowsForLibs()
{
    var libs = new[]
    {
        "aspnet-core",
        "bullseye",
        "configuration",
        "github",
        "hosting",
        "lambda",
        "pulumi",
        "system-extensions",
        "testing"
    };

    foreach (var lib in libs)
    {
        var workflow = new Workflow($"{lib}-ci");

        var paths = new[] { $".github/workflows/{lib}-**", $"libs/{lib}**", "build/**" };

        workflow.On
            .PullRequest()
            .Paths(paths);

        workflow.On
            .Push()
            .Branches("main")
            .Paths(paths)
            .Tags($"{lib}-**");

        var buildJob = workflow
            .Job("build")
            .RunsOn(GitHubHostedRunners.UbuntuLatest)
            .Env(new Dictionary<string, string>
                {
                    { "GITHUB_TOKEN", "${{secrets.GITHUB_TOKEN}}"}
                });

        buildJob.Step().ActionsCheckout();

        buildJob.Step().LogIntoGitHubContainerRegistry();

        buildJob.Step().PrintEnvironment();

        buildJob.Step()
            .Name("Test")
            .Run($"./build.ps1 {lib}-test")
            .Shell(Shells.Pwsh);

        buildJob.Step()
            .Name("Pack")
            .Run($"./build.ps1 {lib}-pack")
            .Shell(Shells.Pwsh);

        buildJob.Step()
            .Name("Push")
            .If("github.event_name == 'push'")
            .Run("./build.ps1 push-github")
            .Shell(Shells.Pwsh);

        buildJob.Step().ActionsUploadArtifact();

        var fileName = $"{lib}-ci";

        WriteWorkflow(workflow, fileName);
    }
}

void GenerateCodeAnalysisWorkflow()
{
    var workflow = new Workflow("CodeQL");

    workflow.On
        .Push()
        .Branches("main");
    workflow.On
        .PullRequest()
        .Branches("main");
    workflow.On
        .Schedule("39 8 * * 1");

    var job = workflow
        .Job("analyze")
        .Name("Analyse")
        .RunsOn("ubuntu-latest")
        .Permissions(
            actions: Permission.Read,
            contents: Permission.Read,
            securityEvents: Permission.Write)
        .Strategy()
        .FailFast(false)
        .Matrix(new Dictionary<string, string[]>
        {
            { "language", new[] { "csharp" } }
        })
        .Job;

    job.Step().ActionsCheckout();

    job.Step()
        .Name("Setup dotnet")
        .With()
            .Key("dotnet-version", "6.0.x");

    job.Step()
        .Run("dotnet --info");

    job.Step()
        .Name("Initialize CodeQL")
        .Uses("github/codeql-action/init@v1")
        .With()
            .Key("languages", "${{ matrix.language }}");

    job.Step()
        .Run("./build.ps1 local build")
        .Shell(Shells.Pwsh);

    job.Step()
        .Name("Perform CodeQL Analysis")
        .Uses("github/codeql-action/analyze@v1");

    WriteWorkflow(workflow, "codeql-analysis");
}

GenerateWorkflowsForLibs();
GenerateCodeAnalysisWorkflow();
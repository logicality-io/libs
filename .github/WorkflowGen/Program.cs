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
        "webhook-relay"
    };

    (string Key, string Value) EnvSecret(string key) => (key, $"${{secrets.{key}}}");

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

        workflow.Permissions(packages: Permission.Write);

        var buildJob = workflow
            .Job("build")
            .RunsOn(GitHubHostedRunners.UbuntuLatest)
            .Env(
                EnvSecret("GITHUB_TOKEN"),
                EnvSecret("LOGICALITY_NUGET_ORG"),
                EnvSecret("WEBHOOKRELAYTOKENKEY"),
                EnvSecret("WEBHOOKRELAYTOKENSECRET"),
                EnvSecret("WEBHOOKURL"));

        buildJob.Step().ActionsCheckout();

        buildJob.Step().LogIntoGitHubContainerRegistry();

        buildJob.Step().ActionsSetupDotNet("8.0.x");

        buildJob.Step().PrintEnvironment();

        buildJob.Step()
            .Name("Test")
            .Run($"./build.ps1 {lib}-test")
            .Shell(Shells.Pwsh)
            .TimeoutMinutes(5);

        buildJob.Step()
            .Name("Pack")
            .Run($"./build.ps1 {lib}-pack")
            .Shell(Shells.Pwsh);

        buildJob.Step()
            .Name("Push to GitHub")
            .If("github.event_name == 'push'")
            .Run("./build.ps1 push-github")
            .ContinueOnError(true)
            .Shell(Shells.Pwsh);

        buildJob.Step()
            .Name("Push to Nuget.org (on tag)")
            .If($"startsWith(github.ref, 'refs/tags/{lib}')")
            .Run("./build.ps1 push-nugetorg")
            .ContinueOnError(true)
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
        .Matrix(("language", new[] { "csharp" }))
        .Job;

    job.Step().ActionsCheckout();

    job.Step().ActionsSetupDotNet("8.0.x");

    job.Step()
        .Run("dotnet --info");

    job.Step()
        .Name("Initialize CodeQL")
        .Uses("github/codeql-action/init@v2")
        .With(("languages", "${{ matrix.language }}"));

    job.Step()
        .Run("./build.ps1 build")
        .Shell(Shells.Pwsh);

    job.Step()
        .Name("Perform CodeQL Analysis")
        .Uses("github/codeql-action/analyze@v2");

    WriteWorkflow(workflow, "codeql-analysis");
}

GenerateWorkflowsForLibs();
GenerateCodeAnalysisWorkflow();
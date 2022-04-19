using Xunit;
using Xunit.Abstractions;

namespace Logicality.GitHub.Actions.Workflow;

public class Example
{
    private readonly ITestOutputHelper _outputHelper;

    public Example(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public void ReadMeExample()
    {
        var workflow = new Workflow("my-workflow");

        workflow.On
            .Push()
            .Branches("main");


        var buildJob = workflow
            .Job("build")
            .RunsOn(GitHubHostedRunner.UbuntuLatest);

        buildJob.Step().ActionsCheckout();

        buildJob.Step()
            .Name("Build")
            .Run("./build.ps1")
            .ShellPowerShell();

        var yaml = workflow.GetYaml();
        var fileName = "my-workflow.yaml";

        _outputHelper.WriteLine(yaml);
    }
}
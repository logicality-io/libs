namespace Logicality.GitHub.Actions.Workflow;

public class Example
{
    public void ReadMeExample()
    {
        var workflow = new Workflow("my-workflow");

        workflow.On
            .Push()
            .Branches("main");


        var buildJob = workflow
            .Job("build")
            .RunsOn(GitHubHostedRunners.UbuntuLatest);

        buildJob.Step().ActionsCheckout();

        buildJob.Step()
            .Name("Build")
            .Run("./build.ps1")
            .Shell(Shells.Pwsh);

        var fileName = "my-workflow.yaml";
        var filePath = $"../workflows/{fileName}";
        workflow.WriteYaml(filePath);
    }
}
namespace Logicality.GitHub.Actions.Workflow;

public static class StepBuilderExtensions
{
    public static Step ShellBash(this Step step)
    {
        step.Shell("bash");
        return step;
    }

    public static Step ShellPowerShell(this Step step)
    {
        step.Shell("pwsh");
        return step;
    }
}
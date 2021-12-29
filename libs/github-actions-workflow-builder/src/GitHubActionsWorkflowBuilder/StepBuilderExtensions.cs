namespace Logicality.GitHubActionsWorkflowBuilder;

public static class StepBuilderExtensions
{
    public static IStepBuilder ShellBash(this IStepBuilder step)
    {
        step.Shell("bash");
        return step;
    }

    public static IStepBuilder ShellPowerShell(this IStepBuilder step)
    {
        step.Shell("pwsh");
        return step;
    }
}
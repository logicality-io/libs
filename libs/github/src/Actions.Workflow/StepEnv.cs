namespace Logicality.GitHub.Actions.Workflow;

public class StepEnv: StepKeyValueMap<StepEnv>
{
    public StepEnv(Step step, IDictionary<string, string> map)
        : base(step, "env", map)
    {
    }
}
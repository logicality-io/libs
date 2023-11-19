namespace Logicality.GitHub.Actions.Workflow;

public class StepEnv(Step step, IDictionary<string, string> map) : StepKeyValueMap<StepEnv>(step, "env", map);
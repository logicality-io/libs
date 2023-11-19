namespace Logicality.GitHub.Actions.Workflow;

public class JobOutputs(Job job, IDictionary<string, string> map) : JobKeyValueMap<JobOutputs>(job, "outputs", map);
namespace Logicality.GitHub.Actions.Workflow;

public class JobEnv(Job job, IDictionary<string, string> map) : JobKeyValueMap<JobEnv>(job, "env", map);
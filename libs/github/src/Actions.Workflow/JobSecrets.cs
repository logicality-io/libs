namespace Logicality.GitHub.Actions.Workflow;

public class JobSecrets(Job job, IDictionary<string, string> map) : JobKeyValueMap<JobSecrets>(job, "secrets", map);
namespace Logicality.GitHub.Actions.Workflow;

public class JobWith(Job job, IDictionary<string, string> map) : JobKeyValueMap<JobWith>(job, "with", map);
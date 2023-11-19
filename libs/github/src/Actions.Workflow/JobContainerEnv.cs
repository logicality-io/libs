namespace Logicality.GitHub.Actions.Workflow;

public class JobContainerEnv(JobContainer jobContainer, IDictionary<string, string> map)
    : JobKeyValueMap<JobContainerEnv>(jobContainer.Job, "env", map)
{
    public JobContainer JobContainer { get; } = jobContainer;
}

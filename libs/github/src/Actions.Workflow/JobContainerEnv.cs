namespace Logicality.GitHub.Actions.Workflow;

public class JobContainerEnv : JobKeyValueMap<JobContainerEnv>
{
    public JobContainer JobContainer { get; }

    public JobContainerEnv(JobContainer jobContainer)
        : base(jobContainer.Job, "env")
    {
        JobContainer = jobContainer;
    }

    public JobContainerEnv(JobContainer jobContainer, IDictionary<string, string> properties)
        : base(jobContainer.Job, "env", properties)
    {
        JobContainer = jobContainer;
    }
}

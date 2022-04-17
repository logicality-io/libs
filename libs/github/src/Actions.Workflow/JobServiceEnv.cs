namespace Logicality.GitHub.Actions.Workflow;

public class JobServiceEnv : JobKeyValueMap<JobServiceEnv>
{
    public JobService Service { get; }

    public JobServiceEnv(JobService jobService)
        : base(jobService.Job, "env")
    {
        Service = jobService;
    }

    public JobServiceEnv(JobService jobService, IDictionary<string, string> properties)
        : base(jobService.Job, "env", properties)
    {
        Service = jobService;
    }
}
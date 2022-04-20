namespace Logicality.GitHub.Actions.Workflow;

public class JobServiceEnv : JobKeyValueMap<JobServiceEnv>
{
    public JobService Service { get; }

    public JobServiceEnv(JobService jobService, IDictionary<string, string> map)
        : base(jobService.Job, "env", map)
    {
        Service = jobService;
    }
}
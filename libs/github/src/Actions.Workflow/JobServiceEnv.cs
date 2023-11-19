namespace Logicality.GitHub.Actions.Workflow;

public class JobServiceEnv(JobService jobService, IDictionary<string, string> map)
    : JobKeyValueMap<JobServiceEnv>(jobService.Job, "env", map)
{
    public JobService Service { get; } = jobService;
}
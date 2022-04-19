namespace Logicality.GitHub.Actions.Workflow;

public class JobEnv: JobKeyValueMap<JobEnv>
{
    public JobEnv(Job job, IDictionary<string, string> properties)
        : base(job, "env", properties)
    {
    }
}
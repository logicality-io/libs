namespace Logicality.GitHub.Actions.Workflow;

public class JobEnv: JobKeyValueMap<JobEnv>
{
    public JobEnv(Job job, IDictionary<string, string> map)
        : base(job, "env", map)
    {
    }
}
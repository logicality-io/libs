namespace Logicality.GitHub.Actions.Workflow;

public class JobSecrets : JobKeyValueMap<JobSecrets>
{
    public JobSecrets(Job job, IDictionary<string, string> map)
        : base(job, "secrets", map)
    {
    }
}
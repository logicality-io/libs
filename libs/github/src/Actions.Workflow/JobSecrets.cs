namespace Logicality.GitHub.Actions.Workflow;

public class JobSecrets : JobKeyValueMap<JobSecrets>
{
    public JobSecrets(Job job, IDictionary<string, string> properties)
        : base(job, "secrets", properties)
    {
    }
}
namespace Logicality.GitHub.Actions.Workflow;

public class JobWith : JobKeyValueMap<JobWith>
{
    public JobWith(Job job, IDictionary<string, string> properties)
        : base(job, "with", properties)
    {
    }
}
namespace Logicality.GitHub.Actions.Workflow;

public class JobWith : JobKeyValueMap<JobWith>
{
    public JobWith(Job job, IDictionary<string, string> map)
        : base(job, "with", map)
    {
    }
}
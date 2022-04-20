namespace Logicality.GitHub.Actions.Workflow;

public class JobOutputs : JobKeyValueMap<JobOutputs>
{
    public JobOutputs(Job job, IDictionary<string, string> map)
        : base(job, "outputs", map)
    {
    }
}
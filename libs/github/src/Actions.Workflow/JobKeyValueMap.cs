namespace Logicality.GitHub.Actions.Workflow;

public abstract class JobKeyValueMap<T> : KeyValueMap<T> where T : JobKeyValueMap<T>
{
    protected JobKeyValueMap(Job job, string nodeName)
        : base(nodeName)
    {
        Job = job;
    }

    protected JobKeyValueMap(Job job, string nodeName, IDictionary<string, string> properties)
        : base(nodeName, properties)
    {
        Job = job;
    }

    /// <summary>
    /// The associated Job
    /// </summary>
    public Job Job { get; }

    /// <summary>
    /// The associated Workflow
    /// </summary>
    public Workflow Workflow => Job.Workflow;
}
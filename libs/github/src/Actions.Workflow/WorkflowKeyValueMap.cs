namespace Logicality.GitHub.Actions.Workflow;

public abstract class WorkflowKeyValueMap<T> : KeyValueMap<T> where T : WorkflowKeyValueMap<T>
{
    protected WorkflowKeyValueMap(Workflow workflow, string nodeName)
        : base(nodeName)
    {
        Workflow = workflow;
    }

    protected WorkflowKeyValueMap(Workflow workflow, string nodeName, IDictionary<string, string> map)
        : base(nodeName, map)
    {
        Workflow = workflow;
    }

    /// <summary>
    /// The associated Workflow
    /// </summary>
    public Workflow Workflow { get; }
}
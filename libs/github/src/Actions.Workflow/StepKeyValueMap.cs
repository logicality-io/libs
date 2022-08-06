namespace Logicality.GitHub.Actions.Workflow;

public abstract class StepKeyValueMap<T> : KeyValueMap<T> where T : StepKeyValueMap<T>
{
    protected StepKeyValueMap(Step step, string nodeName)
        : base(nodeName)
    {
        Step = step;
    }

    protected StepKeyValueMap(Step step, string nodeName, IDictionary<string, string> map)
        : base(nodeName, map)
    {
        Step = step;
    }

    /// <summary>
    /// The associated Step
    /// </summary>
    public Step Step { get; }

    /// <summary>
    /// The associated Job
    /// </summary>
    public Job Job => Step.Job;

    /// <summary>
    /// The associated Workflow
    /// </summary>
    public Workflow Workflow => Job.Workflow;
}
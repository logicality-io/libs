using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

/// <summary>
/// Represents a set of triggers for a workflow.
/// </summary>
public class On
{
    private readonly List<Trigger> _triggers = new();

    internal On(Workflow workflow)
    {
        Workflow = workflow;
    }

    public Workflow Workflow { get; }

    /// <summary>
    /// Trigger a workflow from a 'pull_request' event.
    /// </summary>
    /// <returns></returns>
    public GitTrigger PullRequest()
    {
        var trigger = new GitTrigger("pull_request", this, Workflow);
        _triggers.Add(trigger);
        return trigger;
    }

    /// <summary>
    /// Trigger the workflow from 'pull_request_target' event.
    /// </summary>
    /// <returns></returns>
    public GitTrigger PullRequestTarget()
    {
        var trigger = new GitTrigger("pull_request_target", this, Workflow);
        _triggers.Add(trigger);
        return trigger;
    }

    /// <summary>
    /// Trigger the workflow from a 'push' event.
    /// </summary>
    /// <returns>a <see cref="GitTrigger"/> to configure the trigger.</returns>
    public GitTrigger Push()
    {
        var trigger = new GitTrigger("push", this, Workflow);
        _triggers.Add(trigger);
        return trigger;
    }

    /// <summary>
    /// Trigger the workflow from the specified event.
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns>An <see cref="EventTrigger"/> to confugure the trigger.</returns>
    public EventTrigger Event(string eventName)
    {
        var trigger = new EventTrigger(eventName, this, Workflow);
        _triggers.Add(trigger);
        return trigger;
    }

    /// <summary>
    /// Trigger the workflow on one ore more schedules.
    /// </summary>
    /// <param name="cron"></param>
    /// <returns>The <see cref="On"/> object.</returns>
    public On Schedule(params string[] cron)
    {
        var trigger = new ScheduleTrigger("schedule", cron, this, Workflow);
        _triggers.Add(trigger);
        return this;
    }

    /// <summary>
    /// Trigger the workflow from a 'workflow_call' event.
    /// </summary>
    /// <returns>A <see cref="WorkflowCall"/> to configure the trigger.</returns>
    public WorkflowCall WorkflowCall()
    {
        var trigger = new WorkflowCall(this, Workflow);
        _triggers.Add(trigger);
        return trigger;
    }

    /// <summary>
    /// Trigger the workflow from a 'workflow_run' event.
    /// </summary>
    /// <returns>A <see cref="WorkflowRun"/> to configure the trigger.</returns>
    public WorkflowRun WorkflowRun()
    {
        var trigger = new WorkflowRun(this, Workflow);
        _triggers.Add(trigger);
        return trigger;
    }


    /// <summary>
    /// Trigger the workflow from a 'workflow_dispatch' event.
    /// </summary>
    /// <returns>A <see cref="WorkflowDispatch"/> to configure the trigger.</returns>
    public WorkflowDispatch WorkflowDispatch()
    {
        var trigger = new WorkflowDispatch(this, Workflow);
        _triggers.Add(trigger);
        return trigger;
    }

    internal void Build(YamlMappingNode parent, SequenceStyle sequenceStyle)
    {
        if (_triggers.Any())
        {
            var onNode = new YamlMappingNode();
            foreach (var trigger in _triggers)
            {
                trigger.Build(onNode, sequenceStyle);
            }
            parent.Add("on", onNode);
        }
    }
}
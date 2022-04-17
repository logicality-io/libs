using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class On
{
    private readonly List<Trigger> _triggers = new();

    internal On(Workflow workflow)
    {
        Workflow = workflow;
    }

    public Workflow Workflow { get; }

    public GitTrigger PullRequest()
    {
        var trigger = new GitTrigger("pull_request", this, Workflow);
        _triggers.Add(trigger);
        return trigger;
    }

    public GitTrigger PullRequestTarget()
    {
        var trigger = new GitTrigger("pull_request_target", this, Workflow);
        _triggers.Add(trigger);
        return trigger;
    }

    public GitTrigger Push()
    {
        var trigger = new GitTrigger("push", this, Workflow);
        _triggers.Add(trigger);
        return trigger;
    }

    public Event Event(string eventName)
    {
        var trigger = new Event(eventName, this, Workflow);
        _triggers.Add(trigger);
        return trigger;
    }

    public On Schedule(params string[] cron)
    {
        var trigger = new Schedule("schedule", cron, this, Workflow);
        _triggers.Add(trigger);
        return this;
    }

    public WorkflowCall WorkflowCall()
    {
        var trigger = new WorkflowCall(this, Workflow);
        _triggers.Add(trigger);
        return trigger;
    }

    public WorkflowRun WorkflowRun()
    {
        var trigger = new WorkflowRun(this, Workflow);
        _triggers.Add(trigger);
        return trigger;
    }

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
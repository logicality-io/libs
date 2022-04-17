using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public abstract class Trigger
{
    protected Trigger(string eventName, On @on, Workflow workflow)
    {
        EventName = eventName;
        On = @on;
        Workflow  = workflow;
    }

    public string   EventName { get; }
    public On       On        { get; }
    public Workflow Workflow  { get; }

    internal abstract void Build(YamlMappingNode parent, SequenceStyle sequenceStyle);
}
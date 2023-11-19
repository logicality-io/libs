using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public abstract class Trigger(string eventName, On on, Workflow workflow)
{
    public string   EventName { get; } = eventName;
    public On       On        { get; } = on;
    public Workflow Workflow  { get; } = workflow;

    internal abstract void Build(YamlMappingNode parent, SequenceStyle sequenceStyle);
}
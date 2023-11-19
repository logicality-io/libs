using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class ScheduleTrigger(
    string   eventName,
    string[] crons,
    On       on,
    Workflow workflow)
    : Trigger(eventName, on, workflow)
{
    internal override void Build(YamlMappingNode parent, SequenceStyle sequenceStyle)
    {
        var sequence = new YamlSequenceNode
        {
            Style = sequenceStyle
        };
        foreach (var cron in crons)
        {
            sequence.Add(new YamlMappingNode("cron", new YamlScalarNode(cron){ Style = ScalarStyle.SingleQuoted}));
        }

        parent.Add("schedule", sequence);
    }
}
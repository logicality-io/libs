using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class Event : Trigger
{
    private string[] _types = Array.Empty<string>();

    internal Event(
        string   eventName,
        On       @on,
        Workflow workflow)
        : base(eventName, @on, workflow)
    {
    }

    public Event Types(params string[] types)
    {
        _types = types;
        return this;
    }

    internal override void Build(YamlMappingNode parent, SequenceStyle sequenceStyle)
    {
        var yamlMappingNode = new YamlMappingNode();

        if (_types.Any())
        {
            var sequence = new YamlSequenceNode(_types.Select(s => new YamlScalarNode(s)))
            {
                Style = sequenceStyle
            };
            yamlMappingNode.Add("types", sequence);
        }

        parent.Add(EventName, yamlMappingNode);
    }
}
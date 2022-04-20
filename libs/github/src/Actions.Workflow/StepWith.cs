using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class StepWith
{
    private readonly IDictionary<string, string> _map;
    
    public StepWith(Step step, IDictionary<string, string> map)
    {
        Step = step;
        _map = map;
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

    internal void Build(YamlMappingNode yamlMappingNode, SequenceStyle sequenceStyle)
    {
        if (_map.Any())
        {
            var mappingNode = new YamlMappingNode();
            foreach (var property in _map)
            {
                mappingNode.Add(property.Key, new YamlScalarNode(property.Value));
            }

            yamlMappingNode.Add("with", mappingNode);
        }
    }
}
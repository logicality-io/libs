using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class StepWith(Step step, IDictionary<string, string> map)
{
    /// <summary>
    /// The associated Step
    /// </summary>
    public Step Step { get; } = step;

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
        if (map.Any())
        {
            var mappingNode = new YamlMappingNode();
            foreach (var property in map)
            {
                mappingNode.Add(property.Key, new YamlScalarNode(property.Value));
            }

            yamlMappingNode.Add("with", mappingNode);
        }
    }
}
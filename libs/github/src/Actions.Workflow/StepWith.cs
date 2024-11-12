using YamlDotNet.Core;
using YamlDotNet.Core.Tokens;
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

    internal void Build(YamlMappingNode yamlMappingNode)
    {
        if (map.Any())
        {
            var mappingNode = new YamlMappingNode();
            foreach (var property in map)
            {
                if (property.Value.Contains(Environment.NewLine))
                {
                    var yamlScalarNode = new YamlScalarNode(property.Value)
                    {
                        Style = ScalarStyle.Literal
                    };
                    mappingNode.Add(property.Key, yamlScalarNode);
                }
                else
                {
                    mappingNode.Add(property.Key, property.Value);
                }
            }

            yamlMappingNode.Add("with", mappingNode);
        }
    }
}
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class StepWith
{
    private readonly IDictionary<string, string> _properties = new Dictionary<string, string>();
    
    public StepWith(Step step)
    {
        Step = step;
    }

    public StepWith(Step step, IDictionary<string, string> properties)
    {
        Step        = step;
        _properties = properties;
    }

    /// <summary>
    /// Adds a Key and Values.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="value">The value</param>
    /// <returns></returns>
    public StepWith Key(string key, string value)
    {
        _properties.Add(key, value);
        return this;
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
        if (_properties.Any())
        {
            var mappingNode = new YamlMappingNode();
            foreach (var property in _properties)
            {
                mappingNode.Add(property.Key, property.Value);
            }

            yamlMappingNode.Add("with", mappingNode);
        }
    }
}
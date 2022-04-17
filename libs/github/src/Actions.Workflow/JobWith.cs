using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class JobWith
{
    private readonly IDictionary<string, string> _properties = new Dictionary<string, string>();
    
    public JobWith(Job job)
    {
        Job = job;
    }

    public JobWith(Job job, IDictionary<string, string> properties)
    {
        Job         = job;
        _properties = properties;
    }

    /// <summary>
    /// Adds a Key and Values.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="value">The value</param>
    /// <returns></returns>
    public JobWith Key(string key, string value)
    {
        _properties.Add(key, value);
        return this;
    }

    /// <summary>
    /// The associated Job
    /// </summary>
    public Job Job { get; }

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
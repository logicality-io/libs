using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class Matrix
{
    private readonly IDictionary<string, string[]> _properties = new Dictionary<string, string[]>();
    
    public Matrix(Strategy strategy)
    {
        Strategy = strategy;
    }

    public Matrix(Strategy strategy, IDictionary<string, string[]> properties)
    {
        Strategy    = strategy;
        _properties = properties;
    }

    /// <summary>
    /// Adds a Key and Values.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="values">The values</param>
    /// <returns></returns>
    public Matrix Key(string key, params string[] values)
    {
        _properties.Add(key, values);
        return this;
    }

    /// <summary>
    /// The associated Strategy
    /// </summary>
    public Strategy Strategy { get; }

    /// <summary>
    /// The associated Workflow
    /// </summary>
    public Workflow Workflow => Strategy.Workflow;

    /// <summary>
    /// The associated Job
    /// </summary>
    public Job Job => Strategy.Job;

    internal void Build(YamlMappingNode yamlMappingNode, SequenceStyle sequenceStyle)
    {
        if (_properties.Any())
        {
            var matrixMappingNode = new YamlMappingNode();
            foreach (var property in _properties)
            {
                var values = new YamlSequenceNode { Style = sequenceStyle };
                foreach (var value in property.Value)
                {
                    values.Add(value);
                }
                matrixMappingNode.Add(property.Key, values);
            }

            yamlMappingNode.Add("matrix", matrixMappingNode);
        }
    }
}
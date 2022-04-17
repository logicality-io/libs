using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public abstract class KeyValueMap<T>  where T : KeyValueMap<T>
{
    private readonly string _nodeName;

    protected KeyValueMap(string nodeName)
    {
        _nodeName = nodeName;
    }

    protected KeyValueMap(string nodeName, IDictionary<string, string> properties)
    {
        _nodeName   = nodeName;
        Properties = properties;
    }

    protected IDictionary<string, string> Properties { get; } = new Dictionary<string, string>();

    /// <summary>
    /// Adds a Key and Value.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="value">The value</param>
    /// <returns></returns>
    public T Key(string key, string value)
    {
        Properties.Add(key, value);
        return (T)this;
    }

    internal virtual void Build(YamlMappingNode yamlMappingNode)
    {
        if (Properties.Any())
        {
            var mappingNode = new YamlMappingNode();
            foreach (var property in Properties)
            {
                mappingNode.Add(property.Key, property.Value);
            }

            yamlMappingNode.Add(_nodeName, mappingNode);
        }
    }
}
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public abstract class KeyValueMap<T>  where T : KeyValueMap<T>
{
    private readonly string _nodeName;

    protected KeyValueMap(string nodeName)
    {
        _nodeName = nodeName;
    }

    protected KeyValueMap(string nodeName, IDictionary<string, string> map)
    {
        _nodeName   = nodeName;
        Map = map;
    }

    protected IDictionary<string, string> Map { get; } = new Dictionary<string, string>();

    /// <summary>
    /// Adds a Key and Value.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="value">The value</param>
    /// <returns></returns>
    public T Key(string key, string value)
    {
        Map.Add(key, value);
        return (T)this;
    }

    internal virtual void Build(YamlMappingNode yamlMappingNode)
    {
        if (Map.Any())
        {
            var mappingNode = new YamlMappingNode();
            foreach (var property in Map)
            {
                mappingNode.Add(property.Key, property.Value);
            }

            yamlMappingNode.Add(_nodeName, mappingNode);
        }
    }
}
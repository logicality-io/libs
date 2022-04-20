namespace Logicality.GitHub.Actions.Workflow;

internal static class MapExtensions
{
    internal static IDictionary<string, string> ToDictionary(this (string Key, string Value)[] map) 
        => map.ToDictionary(m => m.Key, m => m.Value);

    internal static IDictionary<string, string[]> ToDictionary(this (string Key, string[] Values)[] map)
        => map.ToDictionary(m => m.Key, m => m.Values);
}
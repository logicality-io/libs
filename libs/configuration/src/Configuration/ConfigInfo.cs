using System.Text;

namespace Logicality.Extensions.Configuration;

/// <summary>
/// Represents information about all configuration providers, paths and values.
/// </summary>
public class ConfigInfo
{
    public ConfigInfo(IReadOnlyCollection<ConfigItem> items)
    {
        Items = items;
    }

    public IReadOnlyCollection<ConfigItem> Items { get; }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        foreach (var item in Items)
        {
            stringBuilder.AppendLine(item.ToString());
        }

        return stringBuilder.ToString();
    }
}
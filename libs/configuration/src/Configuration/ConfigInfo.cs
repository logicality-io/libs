using System.Text;

namespace Logicality.Extensions.Configuration;

/// <summary>
/// Represents information about all configuration providers, paths and values.
/// </summary>
public class ConfigInfo(IReadOnlyCollection<ConfigItem> items)
{
    public IReadOnlyCollection<ConfigItem> Items { get; } = items;

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
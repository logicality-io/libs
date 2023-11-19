namespace Logicality.Extensions.Configuration;

/// <summary>
/// Represents a configuration item
/// </summary>
/// <param name="Path">The configuration path.</param>
/// <param name="Value">The configuration value.</param>
/// <param name="Provider">The configuration provider.</param>
public record ConfigItem(string Path, string Value, string Provider)
{
    public override string ToString() 
        => $"{Path} = {Value} ({Provider})";
}
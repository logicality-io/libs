using Microsoft.Extensions.Configuration;

namespace Logicality.Extensions.Configuration;

public class RuntimeConfigurationProvider : ConfigurationProvider
{
    private readonly object _lockObject = new();

    public override void Set(string key, string? value)
    {
        lock (_lockObject)
        {
            base.Set(key, value);
            OnReload();
        }
    }

    public void Remove(string key)
    {
        lock (_lockObject)
        {
            if (Data.ContainsKey(key))
            {
                Data.Remove(key);
                OnReload();
            }
        }
    }
}
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Logicality.Extensions.Configuration;

public static class ConfigurationRootExtensions
{
    /// <summary>
    /// Gets paths, values and source providers from the the configuration root. Useful for logging
    /// a configuration on startup or other debugging scenarios. To prevent accidental leakage
    /// of secret or sensitive values all values are SHA256 hashed.
    /// </summary>
    /// <param name="root">The configuration root.</param>
    /// <returns>A configuration info object.</returns>
    public static ConfigInfo GetConfigInfo(this IConfigurationRoot root) 
        => GetConfigInfo(root, new HashSet<string>());

    /// <summary>
    /// Gets paths, values and source providers from the the configuration root. Useful for logging
    /// a configuration on startup or other debugging scenarios. To prevent accidental leakage
    /// of secret or sensitive values, all values are SHA256 hashed except for paths explicitly
    /// opted-in to.
    /// </summary>
    /// <param name="root">The configuration root.</param>
    /// <param name="nonSensitivePaths">A collection of path whose values are not deemd to be secret or sensitive.</param>
    /// <returns>A configuration info object.</returns>
    public static ConfigInfo GetConfigInfo(this IConfigurationRoot root, ISet<string> nonSensitivePaths)
    {
        void RecurseChildren(
            ICollection<ConfigItem>            infos,
            IEnumerable<IConfigurationSection> children)
        {
            foreach (var child in children)
            {
                var (value, provider) = GetValueAndProvider(root, child.Path);

                if (provider != null)
                {
                    if (nonSensitivePaths.Contains(child.Path))
                    {
                        var configInfo = new ConfigItem(child.Path, value!, provider.ToString()!);
                        infos.Add(configInfo);
                    }
                    else
                    {
                        var       bytes      = Encoding.UTF8.GetBytes(value!);
                        var       hash       = SHA256.HashData(bytes);
                        var       valueHash  = Convert.ToBase64String(hash);
                        var       configInfo = new ConfigItem(child.Path, valueHash, provider.ToString()!);
                        infos.Add(configInfo);
                    }
                }

                RecurseChildren(infos, child.GetChildren());
            }
        }

        var items = new List<ConfigItem>();
        RecurseChildren(items, root.GetChildren());
        return new ConfigInfo(items);
    }
        
    private static (string? value, IConfigurationProvider? provider) GetValueAndProvider(
        IConfigurationRoot root, 
        string             key)
    {
        foreach (var provider in root.Providers.Reverse())
        {
            if (provider.TryGet(key, out var value))
            {
                return (value, provider);
            }
        }

        return (null, null)!;
    }
}
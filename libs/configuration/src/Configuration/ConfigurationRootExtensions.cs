using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Logicality.Extensions.Configuration
{
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
                ICollection<ConfigItem> infos,
                IEnumerable<IConfigurationSection> children)
            {
                foreach (var child in children)
                {
                    var (value, provider) = GetValueAndProvider(root, child.Path);

                    if (provider != null)
                    {
                        if (nonSensitivePaths.Contains(child.Path))
                        {
                            var configInfo = new ConfigItem(child.Path, value, provider.ToString()!);
                            infos.Add(configInfo);
                        }
                        else
                        {
                            using var sha256 = SHA256.Create();
                            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                            var valueHash = Convert.ToBase64String(hash);
                            var configInfo = new ConfigItem(child.Path, valueHash, provider.ToString()!);
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
        
        private static (string Value, IConfigurationProvider? Provider) GetValueAndProvider(
            IConfigurationRoot root, 
            string key)
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

    /// <summary>
    /// Represents a configuration item.
    /// </summary>
    public class ConfigItem
    {

        /// <summary>
        /// Create a new instance of a config item.
        /// </summary>
        /// <param name="path">The configuration path.</param>
        /// <param name="value">The configuration value.</param>
        /// <param name="provider">The configuration provider.</param>
        public ConfigItem(string path, string value, string provider)
        {
            Path = path;
            Value = value;
            Provider = provider;
        }

        /// <summary>
        /// The configuration path.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The configuration value.
        /// </summary>
        public string Value { get; }
        
        /// <summary>
        /// The configuration provider.
        /// </summary>
        public string Provider { get; }

        public override string ToString() 
            => $"{Path} = {Value} ({Provider})";
    }
}

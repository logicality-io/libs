using System.IO;
using System.Text;
using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extensions over <see cref="IConfigurationBuilder"/>
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        ///     Adds an object to the configuration. The object will first be serialized to json.
        /// </summary>
        /// <param name="config">The configuration builder.</param>
        /// <param name="value">The object to be serialized and added to configuration.</param>
        /// <returns></returns>
        public static IConfigurationBuilder AddObject<T>(this IConfigurationBuilder config, T value) where T:class
        {
            var jsonVersion = JsonSerializer.Serialize(value);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonVersion))
            {
                Position = 0
            };
            return config.AddJsonStream(stream);
        }
    }
}
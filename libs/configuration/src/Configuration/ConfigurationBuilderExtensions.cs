using System.IO;
using System.Text;
using System.Text.Json;
using Logicality.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extensions over <see cref="IConfigurationBuilder"/>
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        ///     Adds an object to the configuration that will first be serialized to json.
        /// </summary>
        /// <param name="config">The configuration builder.</param>
        /// <param name="value">The object to be serialized and added to configuration.</param>
        /// <returns></returns>
        public static IConfigurationBuilder AddJsonStream<T>(this IConfigurationBuilder config, T value) where T:class
        {
            var jsonVersion = JsonSerializer.Serialize(value);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonVersion))
            {
                Position = 0
            };
            return JsonConfigurationExtensions.AddJsonStream(config, stream);
        }

        /// <summary>
        ///     Add a runtime configuration provider that allows setting/opverriding configuration
        ///     at runtime
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="runtimeConfiguration"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddRuntimeConfiguration(
            this IConfigurationBuilder builder,
            RuntimeConfiguration runtimeConfiguration) =>
            builder.Add(new RuntimeConfigurationSource(runtimeConfiguration));
    }
}
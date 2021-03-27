using System.IO;
using System.Text;
using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        ///     Adds an object to the configuration. The object will first be serialized to json.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="value"></param>
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
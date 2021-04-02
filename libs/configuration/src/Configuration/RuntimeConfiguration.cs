namespace Logicality.Extensions.Configuration
{
    /// <summary>
    /// Represents in memory configuration that can be changed at runtime.
    /// </summary>
    public class RuntimeConfiguration
    {
        private RuntimeConfigurationProvider? _configurationProvider;

        internal void SetProvider(RuntimeConfigurationProvider configurationProvider)
            => _configurationProvider = configurationProvider;

        public void SetOverride(string key, string value)
        {
            _configurationProvider!.Set(key, value);
        }

        public void RemoveOverride(string key)
        {
            _configurationProvider!.Remove(key);
        }
    }
}
using Microsoft.Extensions.Configuration;

namespace Logicality.Extensions.Configuration;

public class RuntimeConfigurationSource : IConfigurationSource
{
    private readonly RuntimeConfiguration _runtimeConfiguration;

    public RuntimeConfigurationSource(RuntimeConfiguration runtimeConfiguration)
    {
        _runtimeConfiguration = runtimeConfiguration;
    }

    public IConfigurationProvider Build(IConfigurationBuilder _)
    {
        var provider = new RuntimeConfigurationProvider();
        _runtimeConfiguration.SetProvider(provider);
        return provider;
    }
}
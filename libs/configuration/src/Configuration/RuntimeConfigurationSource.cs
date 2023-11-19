using Microsoft.Extensions.Configuration;

namespace Logicality.Extensions.Configuration;

public class RuntimeConfigurationSource(RuntimeConfiguration runtimeConfiguration) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder _)
    {
        var provider = new RuntimeConfigurationProvider();
        runtimeConfiguration.SetProvider(provider);
        return provider;
    }
}
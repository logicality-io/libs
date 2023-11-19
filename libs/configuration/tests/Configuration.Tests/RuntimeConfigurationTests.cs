using Microsoft.Extensions.Configuration;
using Shouldly;
using Xunit;

namespace Logicality.Extensions.Configuration;

public class RuntimeConfigurationTests
{
    [Fact]
    public void Can_override_value_at_runtime()
    {
        var runtimeConfiguration = new RuntimeConfiguration();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"foo", "bar"}
            })
            .AddRuntimeConfiguration(runtimeConfiguration)
            .Build();
        var value1 = configuration.GetValue<string>("foo");

        runtimeConfiguration.SetOverride("foo", "baz");
        var value2 = configuration.GetValue<string>("foo");

        value2.ShouldBe("baz");
        value2.ShouldNotBe(value1);
    }

    [Fact]
    public void Can_revert_value_at_runtime()
    {
        var runtimeConfiguration = new RuntimeConfiguration();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"foo", "bar"}
            })
            .AddRuntimeConfiguration(runtimeConfiguration)
            .Build();
        runtimeConfiguration.SetOverride("foo", "baz");
        var value1 = configuration.GetValue<string>("foo");

        runtimeConfiguration.RemoveOverride("foo");
        var value2 = configuration.GetValue<string>("foo");

        value2.ShouldBe("bar");
        value2.ShouldNotBe(value1);
    }

    [Fact]
    public void Can_override_json_values_at_runtime()
    {
        var runtimeConfiguration = new RuntimeConfiguration();
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("testappsettings.json", false)
            .AddRuntimeConfiguration(runtimeConfiguration)
            .Build();
        var value1 = configuration.GetValue<string>("serilog:MinimumLevel");

        value1.ShouldBe("Debug");

        runtimeConfiguration.SetOverride("serilog:MinimumLevel", "Info");
        var value2 = configuration.GetValue<string>("serilog:MinimumLevel");

        value2.ShouldBe("Info");
    }
}
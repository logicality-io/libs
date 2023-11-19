using Logicality.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Logicality.Extensions;

public class ConfigurationRootExtensionsTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public void Can_get_config_info()
    {
        var config = new ConfigObject(
            "foo",
            new ConfigObject.Nested("bar"),
            new ConfigObject.Nested("baz"));
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"foo:bar", "a"},
                {"foo:bar:baz", "b"}
            })
            .AddObject(config)
            .AddJsonFile("testappsettings.json", false)
            .Build();

        var configInfo = configuration.GetConfigInfo();

        configInfo.ShouldNotBeNull();

        outputHelper.WriteLine(configInfo.ToString());
    }

    [Fact]
    public void When_supply_non_secret_paths_then_values_should_not_be_hashed()
    {
        var config = new ConfigObject(
            "foo",
            new ConfigObject.Nested("bar"),
            new ConfigObject.Nested("baz"));
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"foo:bar", "a"},
                {"foo:bar:baz", "b"}
            })
            .AddObject(config)
            .AddJsonFile("testappsettings.json", false)
            .Build();

        var nonSecretPaths = new HashSet<string>
        {
            "Field",
            "foo:bar:baz"
        };
        var configInfo = configuration.GetConfigInfo(nonSecretPaths);

        configInfo.Items.Single(i => i.Path == "Field").Value.ShouldBe("foo");
        configInfo.Items.Single(i => i.Path == "foo:bar:baz").Value.ShouldBe("b");
        outputHelper.WriteLine(configInfo.ToString());
    }

    public record ConfigObject(string Field, ConfigObject.Nested Nested1, ConfigObject.Nested Nested2)
    {
        public record Nested(string Field);
    }
}
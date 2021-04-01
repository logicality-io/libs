using System.Collections.Generic;
using System.Linq;
using Logicality.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Logicality.Extensions
{
    public class ConfigurationRootExtensionsTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public ConfigurationRootExtensionsTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void Can_get_config_info()
        {
            var config = new ConfigObject
            {
                Field = "foo",
                Nested1 = new ConfigObject.Nested
                {
                    Field = "bar",
                },
                Nested2 = new ConfigObject.Nested
                {
                    Field = "baz"
                }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"foo:bar", "a"},
                    {"foo:bar:baz", "b"}
                })
                .AddJsonStream(config)
                .AddJsonFile("testappsettings.json", false)
                .Build();

            var configInfo = configuration.GetConfigInfo();

            configInfo.ShouldNotBeNull();

            _outputHelper.WriteLine(configInfo.ToString());
        }

        [Fact]
        public void When_supply_non_secret_paths_then_values_should_not_be_hashed()
        {
            var config = new ConfigObject
            {
                Field = "foo",
                Nested1 = new ConfigObject.Nested
                {
                    Field = "bar",
                },
                Nested2 = new ConfigObject.Nested
                {
                    Field = "baz"
                }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"foo:bar", "a"},
                    {"foo:bar:baz", "b"}
                })
                .AddJsonStream(config)
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
            _outputHelper.WriteLine(configInfo.ToString());
        }

        public class ConfigObject
        {
            public class Nested
            {
                public string Field { get; set; }
            }

            public string Field { get; set; }

            public Nested Nested1 { get; set; }

            public Nested Nested2 { get; set; }
        }
    }
}
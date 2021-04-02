using Microsoft.Extensions.Configuration;
using Shouldly;
using Xunit;

namespace Logicality.Extensions.Configuration
{
    public class ConfigurationBuilderExtensionsTests
    {
        [Fact]
        public void CanAddObjectToConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonStream(new Foo
                {
                    Bar = "baz"
                })
                .Build();
            var value = config.GetValue<string>("Bar");

            value.ShouldBe("baz");
        }

        private class Foo
        {
            public string Bar { get; set; }
        }
    }
}

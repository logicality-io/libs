using Microsoft.Extensions.Configuration;
using Xunit;
using Shouldly;

namespace Logicality.Extensions
{
    public class ConfigurationExtensionsTests
    {
        [Fact]
        public void CanAddObjectToConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddObject(new Foo
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

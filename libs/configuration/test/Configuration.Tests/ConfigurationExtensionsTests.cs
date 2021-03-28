using Microsoft.Extensions.Configuration;
using Shouldly;
using Xunit;

namespace Logicality.Lib
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

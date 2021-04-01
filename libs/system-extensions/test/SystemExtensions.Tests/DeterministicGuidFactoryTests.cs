using System;
using Logicality.SystemExtensions;
using Shouldly;
using Xunit;

namespace Logicality.System
{
    public class DeterministicGuidFactoryTests
    {
        [Fact]
        public void When_same_input_and_same_namespace_then_should_create_same_guid()
        {
            var sut = new DeterministicGuidFactory(Guid.Parse("0E9C9913-06AF-4E24-A7CE-1AB5A06E35D0"));

            var guid1 = sut.Create("foo");

            var guid2 = sut.Create("foo");

            guid2.ShouldBe(guid1);
        }
    }
}

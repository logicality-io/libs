using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Logicality.Testing.Fixtures
{
    public class DynamoDBFixtureTests
    {
        [Fact]
        public async Task Can_run()
        {
            using var dynamoDBFixture = await DynamoDBFixture.Create("testing-fixtures-1");

            dynamoDBFixture.ServiceUrl.ShouldNotBeNull();

            dynamoDBFixture.ServiceUrl.Port.ShouldNotBe(0);
        }

        [Fact]
        public async Task Can_run_in_parallel()
        {
            var fixtures = new ConcurrentBag<DynamoDBFixture>();

            var tasks = new List<Task>();
            for (var i = 0; i < 10; i++)
            {
                var task = Task.Run(async () =>
                {
                    var fixture = await DynamoDBFixture.Create("testing-fixtures-2");
                    fixtures.Add(fixture);
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            foreach (var fixture in fixtures.ToArray())
            {
                fixture.Dispose();
            }
        }
    }
}

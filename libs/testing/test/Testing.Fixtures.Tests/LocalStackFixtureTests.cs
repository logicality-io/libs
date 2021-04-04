using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Logicality.Testing.Fixtures
{
    public class LocalStackFixtureTests
    {
        [Fact]
        public async Task Can_run()
        {
            using var fixture = await LocalstackFixture.Create("testing-fixtures-1", "s3");

            fixture.ServiceUrl.ShouldNotBeNull();
            fixture.ServiceUrl.Port.ShouldNotBe(0);
        }

        [Fact]
        public async Task Can_run_in_parallel()
        {
            var fixtures = new ConcurrentBag<LocalstackFixture>();

            var tasks = new List<Task>();
            for (var i = 0; i < 10; i++)
            {
                var task = Task.Run(async () =>
                {
                    var fixture = await LocalstackFixture.Create("testing-fixtures-2", "s3");
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

using System;
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

            dynamoDBFixture.DynamoDBClient.ShouldNotBeNull();

            var serviceUrl = new Uri(dynamoDBFixture.DynamoDBClient.Config.ServiceURL);
            serviceUrl.Port.ShouldNotBe(0);
        }

        [Fact]
        public async Task Can_run_in_parallel()
        {
            var dynamoDBFixtures = new ConcurrentBag<DynamoDBFixture>();

            var tasks = new List<Task>();
            for (var i = 0; i < 10; i++)
            {
                var task = Task.Run(async () =>
                {
                    var dynamoDBFixture = await DynamoDBFixture.Create("testing-fixtures-2");
                    dynamoDBFixtures.Add(dynamoDBFixture);
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            var dbFixtures = dynamoDBFixtures.ToArray();
            foreach (var dynamoDBFixture in dbFixtures)
            {
                dynamoDBFixture.Dispose();
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Logicality.Testing.Fixtures;
using Xunit;

namespace DynamoDBLock.Tests
{
    public class LockTableTests : IAsyncLifetime
    {
        private DynamoDBFixture _fixture;

        [Fact]
        public void Test1()
        {

        }

        public async Task InitializeAsync()
        {
            _fixture = await DynamoDBFixture.Create("locktable");
        }

        public Task DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}

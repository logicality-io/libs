using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Logicality.System.Net.Sockets
{
    public class PortFinderTests
    {
        [Fact]
        public async Task Parallel_calls_should_all_get_different_ports()
        {
            var ports = new ConcurrentDictionary<int, object>();
            var tasks = new List<Task>();
            for (var i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var port = PortFinder.GetNext();
                    ports.AddOrUpdate(port, _ => null, (_, _) => null);
                }));
            }

            await Task.WhenAll(tasks);

            ports.Count.ShouldBe(10);
        }
    }
}

﻿using System.Collections.Concurrent;
using Logicality.SystemExtensions.Net.Sockets;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Logicality.System.Net.Sockets;

public class PortFinderTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public async Task Parallel_calls_should_all_get_different_ports()
    {
        var ports = new ConcurrentDictionary<int, object>();
        var tasks = new List<Task>();
        for (var i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var port = PortFinder.GetNext();
                ports.AddOrUpdate(port, _ => null, (_, _) => null);
                outputHelper.WriteLine(port.ToString());
            }));
        }

        await Task.WhenAll(tasks);

        ports.Count.ShouldBe(5);
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;

namespace Logicality.Extensions.Hosting.Example
{
    /// <summary>
    /// This class holds references to hosted service. As hosted services start, they register themselves
    /// in this class. This is to facilitate using generated configuration values which may not be
    /// known until *after* a hosted service has started to be retrievable by other hosted services.
    ///
    /// For example, services that log to Seq will need to start after Seq has started and determined
    /// the port number it is listening on.
    /// </summary>
    public class HostedServiceContext
    {
        private readonly Dictionary<string, IHostedService> _hostedServices = new();

        public MySqlHostedService MySql
        {
            get => Get<MySqlHostedService>(nameof(MySql));
            set => Add(nameof(MySqlHostedService), value);
        }

        public SeqHostedService Seq
        {
            get => Get<SeqHostedService>(nameof(Seq));
            set => Add(nameof(Seq), value);
        }

        public MainWebAppHostedService MainWebApp
        {
            get => Get<MainWebAppHostedService>(nameof(MainWebApp));
            set => Add(nameof(MainWebAppHostedService), value);
        }

        public AdminWebAppHostedService AdminWebApp
        {
            get => Get<AdminWebAppHostedService>(nameof(AdminWebApp));
            set => Add(nameof(AdminWebAppHostedService), value);
        }

        private void Add(string name, IHostedService hostedService)
        {
            lock (_hostedServices)
            {
                _hostedServices.Add(name, hostedService);
            }
        }

        private T Get<T>(string name) where T : IHostedService
        {
            if (!_hostedServices.TryGetValue(name, out var value))
            {
                throw new InvalidOperationException($"HostedService {name} was not found. Check the hosted services registration sequence.");
            }

            return (T)value;
        }
    }
}

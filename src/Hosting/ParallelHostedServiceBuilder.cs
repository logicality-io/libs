using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Logicality.Extensions.Hosting
{
    public class ParallelHostedServiceBuilder
    {
        private readonly IServiceCollection _services;
        private readonly string _name;
        private readonly List<Func<IServiceProvider, HostedServiceWrapper>> _activators 
            = new List<Func<IServiceProvider, HostedServiceWrapper>>();

        internal ParallelHostedServiceBuilder(IServiceCollection services, string name)
        {
            _services = services;
            _name = name;
        }

        public ParallelHostedServiceBuilder Host<T>() where T : class, IHostedService
        {
            _services.AddTransient<T>();
            _activators.Add(sp =>
            {
                var hostedService = sp.GetRequiredService<T>();
                var logger = sp.GetRequiredService<ILogger<HostedServiceWrapper>>();
                return new HostedServiceWrapper(hostedService, logger);
            });
            return this;
        }

        public ParallelHostedServiceBuilder HostSequential(string name, Action<SequentialHostedServiceBuilder> sequential)
        {
            var builder = new SequentialHostedServiceBuilder(_services, name);
            sequential(builder);
            _activators.Add(sp => builder.Build(sp));
            return this;
        }

        public ParallelHostedServiceBuilder HostParallel(string name, Action<ParallelHostedServiceBuilder> sequential)
        {
            var builder = new ParallelHostedServiceBuilder(_services, name);
            sequential(builder);
            _activators.Add(sp => builder.Build(sp));
            return this;
        }

        public HostedServiceWrapper Build(IServiceProvider sp)
        {
            var hostedServices = _activators
                .Select(a => a(sp))
                .ToArray();

            var parallelHostedServices = new ParallelHostedServices(hostedServices);
            var logger = sp.GetRequiredService<ILogger<HostedServiceWrapper>>();

            var wrapper = new HostedServiceWrapper(parallelHostedServices, logger)
            {
                Name = _name
            };
            parallelHostedServices.SetParent(wrapper);

            return wrapper;
        }
    }
}

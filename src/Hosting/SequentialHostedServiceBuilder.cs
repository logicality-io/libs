using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Logicality.Extensions.Hosting
{
    public class SequentialHostedServiceBuilder
    {
        private readonly IServiceCollection _services;
        private readonly string _name;
        private readonly List<Func<IServiceProvider, HostedServiceWrapper>> _activators
            = new List<Func<IServiceProvider, HostedServiceWrapper>>();

        internal SequentialHostedServiceBuilder(IServiceCollection services, string name)
        {
            _services = services;
            _name = name;
        }

        /// <summary>
        /// Host a <see cref="IHostedService"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SequentialHostedServiceBuilder Host<T>() where T: class, IHostedService
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

        public SequentialHostedServiceBuilder HostSequential(string name, Action<SequentialHostedServiceBuilder> configure)
        {
            var builder = new SequentialHostedServiceBuilder(_services, name);
            configure(builder);
            _activators.Add(sp => builder.Build(sp));
            return this;
        }

        public SequentialHostedServiceBuilder HostParallel(string name, Action<ParallelHostedServiceBuilder> configure)
        {
            var builder = new ParallelHostedServiceBuilder(_services, name);
            configure(builder);
            _activators.Add(sp => builder.Build(sp));
            return this;
        }

        internal HostedServiceWrapper Build(IServiceProvider sp)
        {
            var hostedServices = _activators
                .Select(a => a(sp))
                .ToArray();
            var sequentialHostedServices = new SequentialHostedServices(hostedServices);
            var logger = sp.GetRequiredService<ILogger<HostedServiceWrapper>>();

            var wrapper = new HostedServiceWrapper(sequentialHostedServices, logger)
            {
                Name = _name
            };
            sequentialHostedServices.SetParent(wrapper);

            return wrapper;
        }
    }
}

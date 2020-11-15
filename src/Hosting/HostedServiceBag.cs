using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;

namespace Logicality.Extensions.Hosting
{
    /// <summary>
    /// Represents a collection of hosted services keyed by name. Used in context
    /// that will prevent duplicate service being started and nice error messages if
    /// there is an attempt to access a hosted service that has not yet started.
    /// </summary>
    public class HostedServiceBag
    {
        private readonly Dictionary<string, IHostedService> _hostedServices = new Dictionary<string, IHostedService>();

        /// <summary>
        /// Adds a hosted service 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hostedService"></param>
        public void Add(string name, IHostedService hostedService)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            _ = hostedService ?? throw new ArgumentNullException(nameof(hostedService));

            _hostedServices.Add(name, hostedService);
        }

        public T Get<T>(string name) where T : IHostedService
        {
            if (!_hostedServices.TryGetValue(name, out var value))
            {
                throw new InvalidOperationException($"HostedService {name} was not found. Perhaps it hasn't been started yet?");
            }

            return (T)value;
        }
    }
}

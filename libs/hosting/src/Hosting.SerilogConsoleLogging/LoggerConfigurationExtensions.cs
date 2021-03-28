using Serilog;
using Serilog.Filters;
using Serilog.Sinks.SystemConsole.Themes;

namespace Logicality.Extensions.Hosting
{
    public static class LoggerConfigurationExtensions
    {
        /// <summary>
        ///     Write HostedService log messages to console with in a nice format. Intended to be used by DevServers.
        /// </summary>
        /// <param name="configuration">
        ///     The Serilog LoggerConfiguration
        /// </param>
        /// <returns>
        ///     The Serilog LoggerConfiguration
        /// </returns>
        public static LoggerConfiguration WriteHostedServiceMessagesToConsole(this LoggerConfiguration configuration)
        {
            configuration
                .Filter
                .ByIncludingOnly(e => Matching.FromSource(typeof(HostedServiceWrapper).FullName)(e))
                .WriteTo
                .Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] [{HostedService}] {Message}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code);
            return configuration;
        }
    }
}

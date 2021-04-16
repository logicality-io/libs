# Lambda

## FunctionBase

An opinionated abstract class that encapsulates configuration, logging and
dependency injection boiler plate code.

- Configures the configuration builder with following providers:
  - Json file: `appsettings.json`, `appsettings.{environment}.json`.
  - Environment variables.
- Bind configuration to your specified configuration object.
- Configures logging with `Amazon.Lambda.Logging.AspNetCore` (which technically
  has nothing to do with ASP.NET core, but is just an extension to
  `Microsoft.Logging.Extensions`).
- Configures the service provider
  - Adds the configuration object with lifetime singleton.
  - Adds the handler type with lifetime transient.

Actions can be passed through the constructor to further configure the function
as needed.

Example usage (See `Lambda.Example` project for complete ):

```csharp
    public class ExampleFunction: FunctionBase<FunctionConfig, Handler>
    {
        public ExampleFunction() 
            : base(ConfigureConfiguration, ConfigureLogging, ConfigureServices)
        { }

        private static void ConfigureConfiguration(IConfigurationBuilder configuration)
        {
            // Additional Confogiration. 
        }

        private static void ConfigureLogging(ILoggingBuilder logging)
        {
            // Additional Logging configuration. 
        }

        private static void ConfigureServices(FunctionConfig config, IServiceCollection services)
        {
            // Add services here.
        }

        public string? Handle(string input, ILambdaContext context)
        {
            // Resolve the handler class and invoke the handler.
            var handler = ServiceProvider.GetRequiredService<Handler>();
            return handler.Handle(input, context);
        }
    }
``csharp

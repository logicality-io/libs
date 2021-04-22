# Lambda Libraries

## 1. Logicality.Lambda

### FunctionBase

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
```

## 2. Logicality.Lambda.ClientExtensions

## 3. Logicality.Lambda.TestHost

A .NET implementation of AWS Lambda's Invoke API that can host and execute .NET Lambdas
for simulation, testing, and debugging purposes.

### Packages

| Name | Package | Description |
|---|---|---|
| `Logicality.Lambda.TestHost` | [![feedz.io][p1]][d1] | Main TestHost package. |

##' Using

It works by running a web server that can handle lambda invocation requests,
activate the appropriate lambda class, invoke it's handler (dealing with any
serialization needs) and returning a response, if any.

Given this simple function:

```csharp
public class ReverseStringFunction
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public string Reverse(string input, ILambdaContext context)
    {
        return new string(input.Reverse().ToArray());
    }
}
```

Create a `LambdaTestHostSettings` that supplies a `ILambdaContext` factory. You
can use the supplied `TestLambdaContext` or your own `ILambdaContext`
implementation. Then add one or more `LambdaFunctionInfo`s to the settings.

```csharp
var settings = new LambdaTestHostSettings(() => new TestLambdaContext());

settings.AddFunction(
    new LambdaFunctionInfo(
        nameof(ReverseStringFunction),
        typeof(ReverseStringFunction),
        nameof(ReverseStringFunction.Reverse)).
        reservedConcurrency: 1); // Optional: This will limit the number of concurrent invocations
```

Create an `AmazonLambdaClient`:

```csharp
using var testHost = await LambdaTestHost.Start(_settings);
var awsCredentials = new BasicAWSCredentials("not", "used");
var lambdaConfig = new AmazonLambdaConfig
{
    ServiceURL = testHost.ServiceUrl.ToString(),
    MaxErrorRetry = 0
};
var lambdaClient = new AmazonLambdaClient(awsCredentials, lambdaConfig);
```

Use the client to invoke the lambda:

```
var invokeRequest = new InvokeRequest
{
    InvocationType = InvocationType.RequestResponse,
    Payload = "ReverseMe",
    FunctionName = "ReverseStringFunction",
};
var invokeResponse = lambdaClient.InvokeAsync(invokeRequest);
...
```

### Comparison with AWS .NET Mock Lambda Test Tool

This is not meant to replace the [Lambda Test Tool][lambda-test-tool] but to augment it.
Key differences are:

- Can work with multiple Lambdas at a time vs Lambda Test Tool's one at a time.
- Is a library you can use in Tests projects or local "development" servers. 
  `Test Host` is useful for developing / debugging multiple lambdas at once 
  (i.e. a Lambda "Application", StepFunctions   etc) and exercising them 
  with code. Lambda Test Tool is GUI and manual.
- `Test Host` uses direct references so any dependencies will not be isolated. Lambda Test Tool
  uses `AssemblyLoadContext` to prevent version conflicts between the tool and your code. 
- You can use AWSSDK Lambda client to your invoke functions.

Like Lambda Test Tool, Test Host is not a local Lambda Environment and thus "not
intended to diagnose platform specific issue but instead it can be useful for
debugging application logic issues.".

### Using with Step Functions Local

- See [Step Functions(https://docs.aws.amazon.com/step-functions/latest/dg/sfn-local.html) docs. 
- See an [`integration test`](test/Lambda.TestHost.Tests/StepFunctionsIntegrationTests.cs) for an example
  that uses Step Functions container with `LAMBDA_ENDPOINT` configured to call back to Lambda Test Host.

### Using with LocalStack

- See [Stepfunctions Local](https://docs.aws.amazon.com/step-functions/latest/dg/sfn-local.html) docs.
- See an [`integration test`](test/Lambda.TestHost.Tests/StepFunctionsIntegrationTests.cs) for an example
  that uses LocalStack container with `LAMBDA_FORWARD_URL` configued to call back to Lambda Test Host.

[p1]: https://img.shields.io/badge/endpoint.svg?url=https%3A%2F%2Ff.feedz.io%2Flogicality%2Fpublic%2Fshield%2FLogicality.AWS.Lambda.TestHost%2Fstable
[d1]: https://f.feedz.io/logicality/public/nuget/index.json
[lambda-test-tool]: https://github.com/aws/aws-lambda-dotnet/tree/master/Tools/LambdaTestTool

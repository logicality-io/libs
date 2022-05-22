# Lambda Libraries

![Nuget](https://img.shields.io/nuget/v/Logicality.Lambda?label=Logicality.Lambda&style=flat-square) 
![Nuget](https://img.shields.io/nuget/v/Logicality.Lambda.ClientExtensions?label=Logicality.Lambda.ClientExtensions&style=flat-square).
![Nuget](https://img.shields.io/nuget/v/Logicality.Lambda.TestHost?label=Logicality.Lambda.TestHost&style=flat-square)

<!-- TOC depthFrom:2 updateOnSave:true -->

- [1. Logicality.Lambda](#1-logicalitylambda)
    - [1.1 Synchronously Invoked Functions](#11-synchronously-invoked-functions)
    - [1.2 Asynchronously Invoked Functions](#12-asynchronously-invoked-functions)
- [2. Logicality.Lambda.ClientExtensions](#2-logicalitylambdaclientextensions)
- [3. Logicality.Lambda.TestHost](#3-logicalitylambdatesthost)
    - [Using](#using)
    - [Comparison with AWS .NET Mock Lambda Test Tool](#comparison-with-aws-net-mock-lambda-test-tool)
    - [Using with Step Functions Local](#using-with-step-functions-local)
    - [Using with LocalStack](#using-with-localstack)
- [Licence and Contributing](#licence-and-contributing)

<!-- /TOC -->

## 1. Logicality.Lambda

A mini framework for defining lambda functions leveraging .NET configuration,
logging and dependency injection.

There are two types of lambda invocation mechanisms:

- [Synchronous](https://docs.aws.amazon.com/lambda/latest/dg/invocation-sync.html):
  the lambda runtime waits for completion and returns a response to the
  invoker.
- [Asynchronous](https://docs.aws.amazon.com/lambda/latest/dg/invocation-async.html):
  lambda runtime puts the request on a queue and returns a success indication to the caller.

With this framework, you can define handlers for the above invocation types and
functions that setup the configuration, logging and dependency injection.

The general approach is

1. Define Input and Response types.
2. Define an Options type to bind to configuration.
3. Define a handler that will be resolved from DI.
4. Define a function to configure logging, configuration and services.

Note the handler method itself is asynchronous .NET perspective in both cases
returning a `Task<T>` or a `Task`.

### 1.1 Synchronously Invoked Functions

Create an options type that holds configuration values you want use in your handler:

```csharp
public class ExampleOptions
{
    public int Timeout { get; set; } = 30;
}
```

Define your input and response types. One can also use primitives, such as
`string` and `int`, on the handler so this is optional:

```csharp
public class Input
{
    public string Url { get; set; }
}

public class Response
{
    public string Body {get; set;}
}
```

Define a handler inheriting from `SynchronousInvokeHandler` supplying the
request, response and options types. Alternatively one can implement the
interface `ISynchronousInvokeHandler`.

```csharp
public class ExampleSynchronousInvokeHandler: SynchronousInvokeHandler<Input, Response, ExampleOptions>
{
    public ExampleSynchronousInvokeHandler(IOptionsSnapshot<ExampleOptions> optionsSnapshot) 
        : base(optionsSnapshot)
    { }

    public override async Task<Response> Handle(Input input, ILambdaContext context)
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(Options.Timeout);
        var response = await httpClient.GetAsync(input.Url);
        var body     = await response.Content.ReadAsStringAsync();
        return new Response
        {
            Body = body
        };
    }
}
```

Define a function that will wire up configuration, logging and services. The
handler is automatically registered in the service collection. You can customise
 the behaviour by overrriding `ConfigureConfiguration`, `ConfigureLogging` and
`ConfigureServices`.

```csharp
public class ExampleSynchronousInvokeFunction
  : SynchronousInvokeFunction<Input, Response, ExampleOptions, ExampleSynchronousInvokeHandler>
{
    protected override void ConfigureConfiguration(IConfigurationBuilder configuration)
    {
        base.ConfigureConfiguration(configuration);
        configuration.AddSecretsManager();
    }

    protected override void ConfigureLogging(ILoggingBuilder logging)
    {
        base.ConfigureLogging(logging);
        logging.SetMinimumLevel(LogLevel.Debug);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddHttpClient();
    }
}
```

By default, the base implementations of these methods perform the following:

- `ConfigureConfiguration` will add JsonFile (appsettings.jon & appsettings.{environment}.json) and
  EnvironmentVariables configuration providers.
- `ConfigureLogging` will add LambdaLogger provider from
  `Amazon.Lambda.Logging.AspNetCore` (not actually related to AspNetCore but
  actually base on Microsoft.Extensions.Logging).

### 1.2 Asynchronously Invoked Functions

Asynchronously invoked functions are essentially the same as Synchronously
Invoked Functions above but without a response type.

```csharp
public class ExampleAsynchronousInvokeHandler : AsynchronousInvokeHandler<Input, ExampleOptions>
{
    private readonly IHttpClientFactory _clientFactory;

    public ExampleAsynchronousInvokeHandler(
      IHttpClientFactory clientFactory,
      IOptionsSnapshot<ExampleOptions> optionsSnapshot) 
        : base(optionsSnapshot)
    {
        _clientFactory = clientFactory;
    }

    public async Task Handle(Input input, ILambdaContext context)
    {
        var httpClient = _clientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(Options.Timeout);
        await httpClient.GetAsync(input.Url);
    }
}

public class ExampleAsynchronousInvokeFunction 
    : AsynchronousInvokeFunction<Input, ExampleOptions, ExampleAsynchronousInvokeHandler>
{
    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddHttpClient();
    }
}
```

## 2. Logicality.Lambda.ClientExtensions

Some helper extension methods around `IAmazonLambda`.

## 3. Logicality.Lambda.TestHost

A .NET implementation of AWS Lambda's Invoke API that can host and execute .NET Lambdas
for simulation, testing, and debugging purposes.

### Using

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

```csharp
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

- See [Step Functions](https://docs.aws.amazon.com/step-functions/latest/dg/sfn-local.html) docs.
- See an [`integration test`](test/Lambda.TestHost.Tests/StepFunctions/StepFunctionsIntegrationTests.cs) for an example
  that uses Step Functions container with `LAMBDA_ENDPOINT` configured to call back to Lambda Test Host.

### Using with LocalStack

- See [Stepfunctions Local](https://docs.aws.amazon.com/step-functions/latest/dg/sfn-local.html) docs.
- See an [`integration test`](test/Lambda.TestHost.Tests/LocalStack/LocalStackIntegrationsTests.cs) for an example
  that uses LocalStack container with `LAMBDA_FORWARD_URL` configured to call back to Lambda Test Host.

[lambda-test-tool]: https://github.com/aws/aws-lambda-dotnet/tree/master/Tools/LambdaTestTool

## Licence and Contributing

Licence is MIT

- Please open a discussion if you have a question or a feature request.
- Please create an issue if there is a bug with a full reproducible.

Generally though, the purpose of this is primarily for Logicality's use cases
and clients. Activities will be prioritized around such. Feel free to copy/fork/
vendorise if you don't want to take a dependency.

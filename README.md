# HostedServices Extensions

The standard mechanism to run a background task is to use [`IHostedService`][hosted-service].
The standard activation mechanism is that each hosted service starts up
sequentially. This can take some time if there are many service and/or if any of
them are slow.

## Packages

| Name | Package | Description |
|---|---|---|
| `Logicality.Extensions.Hosting`                      | [![feedz.io][p1]][d1] | Main package. |
| `Logicality.Extensions.Hosting.Docker`               | [![feedz.io][p2]][d2] | Docker specific hosted service helper. |
| `Logicality.Extensions.Hosing.SerilogConsoleLogging` | [![feedz.io][p3]][d3] | Opinionated console logger. |


## Using

Using standard mechanism, these service will start one after the other:

```csharp

var hostBuilder = new HostBuilder()
    .ConfigureServices(services =>
    {
        services.AddHostedService<Foo>();
        services.AddHostedService<Bar>();
        services.AddHostedService<Baz>();
    })

```

With this library, these services can be started in parallel:

```csharp

services.AddParallelHostedServices("webapps", w => w
    .Host<Foo>()
    .Host<Bar>()
    .Host<Baz>());

```

It's possible to combine sequential and parallel hosted services in a hierarchy:

```csharp

services.AddSequentialHostedServices("root", w => w
    .HostParallel("containers",  // the name is used for logging.
        p => p
            .Host<LocalStack>()
            .Host<MySql>()));
    .HostParallel("webapps", 
        cc => pc
            .Host<Admin>()
            .Host<Api>());

```

See [runnable example project][example-project] for a bigger example.

---

[hosted-service]: https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice
[p1]: https://img.shields.io/badge/endpoint.svg?url=https%3A%2F%2Ff.feedz.io%2Flogicality%2Fpublic%2Fshield%2FLogicality.Extensions.Hosting%2Fstable
[d1]: https://f.feedz.io/logicality/public/packages/Logicality.Extensions.Hosting/stable/download
[p2]: https://img.shields.io/badge/endpoint.svg?url=https%3A%2F%2Ff.feedz.io%2Flogicality%2Fpublic%2Fshield%2FLogicality.Extensions.Hosting.Docker%2Fstable
[d2]: https://f.feedz.io/logicality/public/packages/Logicality.Extensions.Docker/stable/download
[p3]: https://img.shields.io/badge/endpoint.svg?url=https%3A%2F%2Ff.feedz.io%2Flogicality%2Fpublic%2Fshield%2FLogicality.Extensions.Hosting.SerilogConsoleLogging%2Fstable
[d3]: https://f.feedz.io/logicality/public/packages/Logicality.Extensions.Hosting.SerilogConsoleLogging/stable/download
[example-project]: /src/Hosting.Example
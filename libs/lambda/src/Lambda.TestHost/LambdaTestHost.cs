using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Logicality.AWS.Lambda.TestHost
{
    /// <summary>
    /// 
    /// </summary>
    public class LambdaTestHost: IAsyncDisposable
    {
        private readonly IWebHost _webHost;

        private LambdaTestHost(LambdaTestHostSettings settings, IWebHost host, Uri serviceUrl)
        {
            _webHost = host;
            Settings = settings;
            ServiceUrl = serviceUrl;
        }

        public LambdaTestHostSettings Settings { get; }

        /// <summary>
        /// The URL that the LambdaTestHost will handle invocation requests.
        /// </summary>
        public Uri ServiceUrl { get; }

        public static async ValueTask<LambdaTestHost> Start(LambdaTestHostSettings settings)
        {
            var host = WebHost
                .CreateDefaultBuilder<Startup>(Array.Empty<string>())
                .UseUrls(settings.WebHostUrl)
                .ConfigureLogging(settings.ConfigureLogging)
                .ConfigureServices(services =>
                {
                    services.AddSingleton(settings);
                })
                .Build();

            await host.StartAsync();

            var serviceUrl = host.GetUris().Single();

            return new LambdaTestHost(settings, host, serviceUrl);
        }

        private class Startup
        {
            private readonly LambdaTestHostSettings _settings;
            private readonly LambdaAccountPool _lambdaAccountPool;

            public Startup(LambdaTestHostSettings settings)
            {
                _settings = settings;
                _lambdaAccountPool = new LambdaAccountPool(
                    settings.AccountConcurrencyLimit,
                    settings.Functions);
            }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddRouting();
            }

            public void Configure(IApplicationBuilder app)
            {
                app.UseRouting();

                app.Use((context, next) =>
                {
                    if (context.Request.Method == "POST" && context.Request.Headers.ContainsKey("lambda-function-name"))
                    {
                        var functionName = context.Request.Headers["lambda-function-name"];
                        context.Request.Path = $"/2015-03-31/functions/{functionName}/invocations";
                        return next();
                    }

                    return next();
                });

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapPost("/2015-03-31/functions/{functionName}/invocations", HandleInvocation);
                });
            }

            private async Task HandleInvocation(HttpContext ctx)
            {
                var logger = ctx.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger<LambdaTestHost>();
                var functionName = (string)ctx.Request.RouteValues["functionName"]!;
                if (!_settings.Functions.TryGetValue(functionName, out var lambdaFunction))
                {
                    ctx.Response.StatusCode = 404;
                    return;
                }

                try
                {
                    using var streamReader = new StreamReader(ctx.Request.Body, Encoding.UTF8);
                    var payload = await streamReader.ReadToEndAsync();

                    var lambdaInstance = _lambdaAccountPool.Get(functionName);
                    if (lambdaInstance == null)
                    {
                        ctx.Response.StatusCode = 429;
                        return;
                    }

                    var settings = ctx.RequestServices.GetRequiredService<LambdaTestHostSettings>();

                    var context = settings.CreateContext();

                    var parameters = BuildParameters(lambdaFunction, context, payload);

                    _settings.InvocationOnStart.Set();
                    var lambdaReturnObject = lambdaFunction.HandlerMethod.Invoke(lambdaInstance!.FunctionInstance, parameters);
                    var responseBody = await ProcessReturnAsync(lambdaFunction, lambdaReturnObject);

                    ctx.Response.StatusCode = 200;
                    if (responseBody != null)
                    {
                        await ctx.Response.WriteAsync(responseBody);
                    }

                    _lambdaAccountPool.Return(lambdaInstance);
                }
                catch (TargetInvocationException ex)
                {
                    logger.LogError(ex, "Error invoking function");
                    ctx.Response.StatusCode = 500;
                    // TODO response body error
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error invoking function");
                    ctx.Response.StatusCode = 500;
                    // TODO response body error
                }
            }

            /// Adapted from Amazon.Lambda.TestTools
            private static object[] BuildParameters(LambdaFunctionInfo functionInfo, ILambdaContext context, string? payload)
            {
                var parameters = functionInfo.HandlerMethod.GetParameters();
                var parameterValues = new object[parameters.Length];

                if (parameterValues.Length > 2)
                    throw new Exception($"Method {functionInfo.HandlerMethod.Name} has too many parameters, {parameterValues.Length}. Methods called by Lambda" +
                                        $"can have at most 2 parameters. The first is the input object and the second is an ILambdaContext.");

                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType == typeof(ILambdaContext))
                    {
                        parameterValues[i] = context;
                    }
                    else if (payload != null)
                    {
                        var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
                        if (functionInfo.Serializer != null)
                        {
                            var genericMethodInfo = functionInfo.Serializer.GetType()
                                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                .FirstOrDefault(x => string.Equals(x.Name, "Deserialize"));

                            var methodInfo = genericMethodInfo?.MakeGenericMethod(parameters[i].ParameterType);

                            try
                            {
                                parameterValues[i] = methodInfo!.Invoke(functionInfo.Serializer, new object[] { stream })!;
                            }
                            catch (Exception e)
                            {
                                throw new Exception($"Error deserializing the input JSON to type {parameters[i].ParameterType.Name}", e);
                            }
                        }
                        else
                        {
                            parameterValues[i] = stream;
                        }
                    }
                }

                return parameterValues;
            }

            /// Adapted from Amazon.Lambda.TestTools
            private static async Task<string?> ProcessReturnAsync(LambdaFunctionInfo functionInfo, object? lambdaReturnObject)
            {
                Stream? lambdaReturnStream = null;

                if (lambdaReturnObject == null)
                {
                    return null;
                }

                // If the return was a Task then wait till the task is complete.
                if (lambdaReturnObject is Task task)
                {
                    await task;

                    // Check to see if the Task returns back an object.
                    if (task.GetType().IsGenericType)
                    {
                        var resultProperty = task.GetType().GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
                        if (resultProperty != null)
                        {
                            var taskResult = resultProperty.GetMethod!.Invoke(task, null);
                            if (taskResult is Stream stream)
                            {
                                lambdaReturnStream = stream;
                            }
                            else
                            {
                                lambdaReturnStream = new MemoryStream();
                                functionInfo.Serializer!.Serialize(taskResult, lambdaReturnStream);
                            }
                        }
                    }
                }
                else
                {
                    lambdaReturnStream = new MemoryStream();
                    functionInfo.Serializer!.Serialize(lambdaReturnObject, lambdaReturnStream);
                }

                if (lambdaReturnStream == null)
                {
                    return null;
                }

                lambdaReturnStream.Position = 0;
                using var reader = new StreamReader(lambdaReturnStream);
                return await reader.ReadToEndAsync();
            }

            public static string GenerateErrorMessage(Exception? e)
            {
                var sb = new StringBuilder();

                if (e is AggregateException)
                {
                    e = e.InnerException;
                }

                var exceptionDepth = 0;
                while (e != null)
                {
                    if (sb.Length > 0)
                        sb.AppendLine($"---------------- Inner {exceptionDepth} Exception ------------");

                    sb.AppendLine($"{e.GetType().FullName}: {e.Message}");
                    sb.AppendLine(e.StackTrace);

                    e = e.InnerException;
                    exceptionDepth++;
                }

                return sb.ToString();
            }
        }

        public async ValueTask DisposeAsync() => await _webHost.StopAsync();
    }
}
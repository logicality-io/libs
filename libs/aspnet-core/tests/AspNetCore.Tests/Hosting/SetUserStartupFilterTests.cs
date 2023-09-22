using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Logicality.AspNetCore.Hosting;

public class SetUserStartupFilterTests : IAsyncLifetime
{
    private IWebHost                           _webHost = null!;
    private HttpClient                         _client = null!;
    private Func<HttpContext, ClaimsPrincipal> _getUser = null!;

    [Fact]
    public async Task When_user_not_set_then_should_get_unauthorized()
    {
        _getUser = ctx => ctx.User;
            
        var response = await _client.GetAsync("/");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task When_user_set_then_should_get_ok()
    {
        var claims = new[]
        {
            new Claim("scope", "api")
        };
        var claimsIdentity  = new ClaimsIdentity(claims, "cookie");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _getUser = _ => claimsPrincipal;

        var response = await _client.GetAsync("/");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    public async Task InitializeAsync()
    {
        _webHost = WebHost
            .CreateDefaultBuilder(Array.Empty<string>())
            .ConfigureServices(services =>
            {
                services.AddSingleton<IStartupFilter>(new SetUserStartupFilter(ctx => _getUser(ctx)));
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/", ctx =>
                    {
                        if (ctx.User.Identity != null && ctx.User.Identity.IsAuthenticated)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }

                        return Task.CompletedTask;
                    });
                });
            })
            .UseKestrel()
            .UseUrls("http://127.0.0.1:0")
            .Build();

        await _webHost.StartAsync();

        var serverPort = _webHost.GetServerUris().First().Port;

        _client = new HttpClient
        {
            BaseAddress = new Uri($"http://127.0.0.1:{serverPort}")
        };
    }

    public async Task DisposeAsync()
    {
        await _webHost.StopAsync();
        _webHost.Dispose();
    }
}
using ID.Application.Middleware.ExternalPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;

namespace ID.Application.Tests.Middlleware;

public class ExternalPagesAuthIntegrationTests
{
    private const string _apiOkResonse = "api ok";
    private const string _externalOkResonse = "external ok";
    private const string _externalPath = "/myid-jobs-dashboard";
    private const string _apiPath = "/api";
    private const string _authScheme = "Test";

    [Fact]
    public async Task NonExternalRequest_IsNotBlocked()
    {
        using var server = CreateServer();
        var client = server.CreateClient();
        var response = await client.GetAsync(_apiPath);
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        content.ShouldBe(_apiOkResonse);
    }

    //--------------------------//

    [Fact]
    public async Task UnauthenticatedExternalRequest_IsBlockedWith401AndJson()
    {
        using var server = CreateServer();
        var client = server.CreateClient();
        var response = await client.GetAsync(_externalPath);
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");
        content.ShouldContain("Unauthorized");
        response.Headers.WwwAuthenticate.ToString().ShouldContain("Bearer");
    }

    //--------------------------//

    [Fact]
    public async Task CustomPredicate_BlocksOrAllows()
    {
        // Predicate blocks all
        using (var server = CreateServer(ctx => false, addAuth: true))
        {
            var client = server.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authScheme);
            var response = await client.GetAsync(_externalPath);
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
        // Predicate allows all
        using (var server = CreateServer(ctx => true, addAuth: true))
        {
            var client = server.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authScheme);
            var response = await client.GetAsync(_externalPath);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }


    //----------------------------//

    private static TestServer CreateServer(Predicate<HttpContext>? predicate = null, bool addAuth = false)
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                if (addAuth)
                {
                    services.AddAuthentication(_authScheme)
                        .AddScheme<TestAuthSchemeOptions, TestAuthHandler>(_authScheme, options => { }); //Always pass authentication challenge
                }
            })
            .Configure(app =>
            {
                if (addAuth)
                    app.UseAuthentication();

                // Use the middleware under test
                if (predicate is null)
                    app.UseExternalPagesAuth_SuperTeam(_externalPath);
                else
                    app.UseExternalPagesAuth_Custom(_externalPath, predicate);

                app.Map(_externalPath, b => b.Run(async ctx =>
                {
                    await ctx.Response.WriteAsync(_externalOkResonse);
                }));

                app.Map(_apiPath, b => b.Run(async ctx =>
                {
                    await ctx.Response.WriteAsync(_apiOkResonse);
                }));
            });

        return new TestServer(builder);
    }
}

//##########################//


// Minimal test auth handler for simulating authenticated users
public class TestAuthSchemeOptions : AuthenticationSchemeOptions { }


//--------------------------//

public class TestAuthHandler(
    IOptionsMonitor<TestAuthSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<TestAuthSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "TestUser") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

}//Cls

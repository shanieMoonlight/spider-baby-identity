//using ID.AddOns.Middleware.Swagger;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.TestHost;
//using Microsoft.Extensions.DependencyInjection;
//using Shouldly;
//using System.Net;
//using System.Net.Http.Headers;
//using System.Security.Claims;

//namespace ID.AddOns.Tests.Middleware.Swagger;

//public class SwaggerAuthMiddlewareTests
//{

//    private const string _apiOkResonse = "api ok";
//    private const string _swaggerOkResonse = "swagger ok";
//    private const string _swaggerPath = "/swagger";
//    private const string _apiPath = "/api";
//    private const string _authScheme = "Test";

//    [Fact]
//    public async Task NonSwaggerRequest_IsNotBlocked()
//    {
//        using var server = CreateServer();
//        var client = server.CreateClient();
//        var response = await client.GetAsync(_apiPath);
//        var content = await response.Content.ReadAsStringAsync();
//        response.StatusCode.ShouldBe(HttpStatusCode.OK);
//        content.ShouldBe(_apiOkResonse);
//    }

//    [Fact]
//    public async Task UnauthenticatedSwaggerRequest_IsBlockedWith401AndJson()
//    {
//        using var server = CreateServer();
//        var client = server.CreateClient();
//        var response = await client.GetAsync(_swaggerPath);
//        var content = await response.Content.ReadAsStringAsync();
//        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
//        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");
//        content.ShouldContain("Unauthorized");
//        response.Headers.WwwAuthenticate.ToString().ShouldContain("Bearer");
//    }

//    [Fact]
//    public async Task AuthenticatedSwaggerRequest_IsAllowed()
//    {
//        using var server = CreateServer(addAuth: true);
//        var client = server.CreateClient();
//        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authScheme);
//        var response = await client.GetAsync(_swaggerPath);
//        var content = await response.Content.ReadAsStringAsync();
//        response.StatusCode.ShouldBe(HttpStatusCode.OK);
//        content.ShouldBe(_swaggerOkResonse);
//    }

//    [Fact]
//    public async Task CustomPredicate_BlocksOrAllows()
//    {
//        // Predicate blocks all
//        using (var server = CreateServer(ctx => false, addAuth: true))
//        {
//            var client = server.CreateClient();
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authScheme);
//            var response = await client.GetAsync(_swaggerPath);
//            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
//        }
//        // Predicate allows all
//        using (var server = CreateServer(ctx => true, addAuth: true))
//        {
//            var client = server.CreateClient();
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authScheme);
//            var response = await client.GetAsync(_swaggerPath);
//            response.StatusCode.ShouldBe(HttpStatusCode.OK);
//        }
//    }


//    //----------------------------//


//    private static TestServer CreateServer(Predicate<HttpContext>? predicate = null, bool addAuth = false)
//    {
//        var builder = new WebHostBuilder()
//            .ConfigureServices(services =>
//            {
//                if (addAuth)
//                {
//                    services.AddAuthentication(_authScheme)
//                        .AddScheme<TestAuthSchemeOptions, TestAuthHandler>(_authScheme, options => { });
//                }
//            })
//            .Configure(app =>
//            {
//                if (addAuth)
//                    app.UseAuthentication();

//                app.UseSwaggerAuth(predicate);
                
//                app.Map(_swaggerPath, b => b.Run(async ctx =>
//                {
//                    await ctx.Response.WriteAsync(_swaggerOkResonse);
//                }));

//                app.Map(_apiPath, b => b.Run(async ctx =>
//                {
//                    await ctx.Response.WriteAsync(_apiOkResonse);
//                }));
//            });

//        return new TestServer(builder);
//    }
//}


////#####################################################//


//// Minimal test auth handler for simulating authenticated users
//public class TestAuthSchemeOptions : AuthenticationSchemeOptions { }

//// Update the constructor of TestAuthHandler to use ISystemClock instead of TimeProvider
//public class TestAuthHandler(
//    Microsoft.Extensions.Options.IOptionsMonitor<TestAuthSchemeOptions> options,
//    Microsoft.Extensions.Logging.ILoggerFactory logger,
//    System.Text.Encodings.Web.UrlEncoder encoder,
//    ISystemClock clock) : AuthenticationHandler<TestAuthSchemeOptions>(options, logger, encoder, clock)
//{
//    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
//    {
//        var claims = new[] { new Claim(ClaimTypes.Name, "TestUser") };
//        var identity = new ClaimsIdentity(claims, Scheme.Name);
//        var principal = new ClaimsPrincipal(identity);
//        var ticket = new AuthenticationTicket(principal, Scheme.Name);
//        return Task.FromResult(AuthenticateResult.Success(ticket));
//    }
//}

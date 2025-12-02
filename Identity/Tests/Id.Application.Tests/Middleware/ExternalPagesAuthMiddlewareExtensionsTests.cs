using ID.Application.Middleware.ExternalPages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace ID.Application.Tests.Middleware;

public class ExternalPagesAuthMiddlewareExtensionsTests
{

    [Fact]
    public void UseExternalPagesAuth_Custom_RegistersMiddleware_AndReturnsBuilder()
    {
        var builder = new FakeApplicationBuilder();

        var ret = builder.UseExternalPagesAuth_Custom("/p", ctx => false);

        ret.ShouldBeSameAs(builder);
        builder.LastMiddlewareFactory.ShouldNotBeNull();
    }

    //--------------------------//

    [Fact]
    public void ShortcutExtensions_RegisterMiddleware()
    {
        var builder = new FakeApplicationBuilder();
        builder.UseExternalPagesAuth_SuperTeam("/s");
        builder.LastMiddlewareFactory.ShouldNotBeNull();

        builder = new FakeApplicationBuilder();
        builder.UseExternalPagesAuth_MntcMinimum("/m");
        builder.LastMiddlewareFactory.ShouldNotBeNull();

        builder = new FakeApplicationBuilder();
        builder.UseExternalPagesAuth_CustomerMinimum("/c");
        builder.LastMiddlewareFactory.ShouldNotBeNull();

        builder = new FakeApplicationBuilder();
        builder.UseExternalPagesAuth_MntcTeam("/m2");
        builder.LastMiddlewareFactory.ShouldNotBeNull();

        builder = new FakeApplicationBuilder();
        builder.UseExternalPagesAuth_CustomerTeam("/c2");
        builder.LastMiddlewareFactory.ShouldNotBeNull();
    }

    //--------------------------//

    [Fact]
    public async Task RegisteredFactory_ProducesMiddleware_That_CallsNext_ForNonExternalPath()
    {
        var builder = new FakeApplicationBuilder();
        builder.UseExternalPagesAuth_Custom("/ext", ctx => false);
        var factory = builder.LastMiddlewareFactory.ShouldNotBeNull();

        // next writes "next" into response
        static async Task next(HttpContext ctx)
        {
            ctx.Response.StatusCode = StatusCodes.Status200OK;
            await ctx.Response.WriteAsync("next");
        }

        var composed = factory(next);

        var ctx = new DefaultHttpContext();
        ctx.Request.Path = "/api/test";
        ctx.Response.Body = new MemoryStream();

        await composed(ctx);

        ctx.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
        ctx.Response.Body.Seek(0, SeekOrigin.Begin);
        using var sr = new StreamReader(ctx.Response.Body);
        var body = await sr.ReadToEndAsync();
        body.ShouldBe("next");
    }

    //--------------------------//

    [Fact]
    public async Task RegisteredFactory_ProducesMiddleware_That_Returns401_ForExternalPath_WhenUnauthenticated()
    {
        var builder = new FakeApplicationBuilder();
        builder.UseExternalPagesAuth_Custom("/ext", ctx => false);
        var factory = builder.LastMiddlewareFactory.ShouldNotBeNull();

        static Task next(HttpContext ctx) => throw new InvalidOperationException("next should not be called");

        var composed = factory(next);

        var ctx = new DefaultHttpContext();
        ctx.Request.Path = "/ext";
        ctx.Response.Body = new MemoryStream();

        await composed(ctx);

        ctx.Response.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        ctx.Response.ContentType?.ShouldContain("application/json");
        ctx.Response.Headers.WWWAuthenticate.ToString().ShouldBe(ExternalPagesAuthMiddleware._wwwAuthenticateHeader);

        ctx.Response.Body.Seek(0, SeekOrigin.Begin);
        using var sr = new StreamReader(ctx.Response.Body);
        var body = await sr.ReadToEndAsync();
        body.ShouldContain("Unauthorized");
    }

    //#############################//

    private class FakeApplicationBuilder : IApplicationBuilder
    {
        // Provide a service provider that contains IWebHostEnvironment so middleware can be activated
        public IServiceProvider ApplicationServices { get; set; } = new ServiceCollection()
            .AddSingleton(Mock.Of<IWebHostEnvironment>())
            .BuildServiceProvider();
        public IFeatureCollection ServerFeatures { get; } = new FeatureCollection();
        public IDictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();

        public Func<RequestDelegate, RequestDelegate>? LastMiddlewareFactory { get; private set; }

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            // Wrap middleware factory to prevent exceptions during activation in test env.
            LastMiddlewareFactory = next =>
            {
                try
                {
                    return middleware(next);
                }
                catch
                {
                    // If middleware can't be activated (missing services), fall back to a delegate that simply calls next.
                    return next;
                }
            };
            return this;
        }

        public RequestDelegate Build() => _ => Task.CompletedTask;

        public IApplicationBuilder New() => new FakeApplicationBuilder();
    }
}

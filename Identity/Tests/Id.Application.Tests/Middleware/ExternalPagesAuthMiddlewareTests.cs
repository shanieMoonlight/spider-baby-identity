//using ID.Application.Middleware.ExternalPages;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Options;

//namespace ID.Application.Tests.Middlleware;

//#pragma warning disable IDE0039 // Use local function
//public class ExternalPagesAuthMiddlewareTests
//{
//    [Fact]
//    public async Task NonExternalRequest_IsNotBlocked()
//    {
//        var options = new ExternalPagesAuthMiddlewareOptions(null, "/external");
//        var mockOpts = new Mock<IOptions<ExternalPagesAuthMiddlewareOptions>>();
//        mockOpts.SetupGet(x => x.Value).Returns(options);

//        RequestDelegate next = ctx => ctx.Response.WriteAsync("ok");

//        var middleware = new ExternalPagesAuthMiddleware(next, mockOpts.Object);

//        var context = new DefaultHttpContext();
//        context.Request.Path = "/api/values";
//        context.Response.Body = new MemoryStream();

//        await middleware.InvokeAsync(context);

//        context.Response.StatusCode.ShouldBe(200);
//        context.Response.Body.Seek(0, SeekOrigin.Begin);
//        using var sr = new StreamReader(context.Response.Body);
//        var body = await sr.ReadToEndAsync();
//        body.ShouldBe("ok");
//    }


//    //-------------------------//


//    [Fact]
//    public async Task Unauthenticated_ExternalRequest_Returns401Json()
//    {
//        var startPath = "/myid-jobs-dashboard";
//        var options = new ExternalPagesAuthMiddlewareOptions(null, startPath);
//        var mockOpts = new Mock<IOptions<ExternalPagesAuthMiddlewareOptions>>();
//        mockOpts.SetupGet(x => x.Value).Returns(options);

//        RequestDelegate next = ctx => throw new Exception("Next should not be called");

//        var middleware = new ExternalPagesAuthMiddleware(next, mockOpts.Object);

//        var context = new DefaultHttpContext();
//        context.Request.Path = startPath;
//        context.Response.Body = new MemoryStream();

//        await middleware.InvokeAsync(context);

//        context.Response.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
//        context.Response.ContentType?.ShouldContain("application/json");
//        context.Response.Headers.WWWAuthenticate.ToString().ShouldBe("Bearer");

//        context.Response.Body.Seek(0, SeekOrigin.Begin);
//        using var sr = new StreamReader(context.Response.Body);
//        var body = await sr.ReadToEndAsync();
//        body.ShouldContain("Unauthorized");
//        body.ShouldContain(startPath);
//    }


//    //-------------------------//


//    [Fact]
//    public async Task Authenticated_AndPredicateAllows_CallsNext()
//    {
//        var startPath = "/myid-jobs-dashboard";
//        var options = new ExternalPagesAuthMiddlewareOptions(ctx => true, startPath);
//        var mockOpts = new Mock<IOptions<ExternalPagesAuthMiddlewareOptions>>();
//        mockOpts.SetupGet(x => x.Value).Returns(options);

//        RequestDelegate next = ctx => ctx.Response.WriteAsync("allowed");

//        var middleware = new ExternalPagesAuthMiddleware(next, mockOpts.Object);

//        var context = new DefaultHttpContext();
//        context.Request.Path = startPath;
//        context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "t")], "Test"));
//        context.Response.Body = new MemoryStream();

//        await middleware.InvokeAsync(context);

//        context.Response.StatusCode.ShouldBe(200);
//        context.Response.Body.Seek(0, SeekOrigin.Begin);
//        using var sr = new StreamReader(context.Response.Body);
//        var body = await sr.ReadToEndAsync();
//        body.ShouldBe("allowed");
//    }

//}//Cls

//#pragma warning restore IDE0039 // Use local function
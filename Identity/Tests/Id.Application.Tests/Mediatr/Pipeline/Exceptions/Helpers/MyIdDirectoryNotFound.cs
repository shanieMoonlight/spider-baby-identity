using ID.Application.Tests.Mediatr.Pipeline.Exceptions.Logger;
using ID.Domain.Utility.Exceptions;
using MediatR;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Mediatr.Pipeline.Exceptions.Helpers;

//- - - - - - - - - - - - - - - - - - //

public class MyIdDirectoryNotFoundHandler : IRequestHandler<TestExceptionsRequest, BasicResult>
{
    public Task<BasicResult> Handle(TestExceptionsRequest request, CancellationToken cancellationToken) =>
        throw new MyIdDirectoryNotFoundException(request.Message ?? "");
}

//- - - - - - - - - - - - - - - - - - //

public class MyIdDirectoryNotFoundExceptionTestHelper
{
    internal static TestParamaters Params =>
        new(() => MyContainerProvider.GetContainer<MyIdDirectoryNotFoundHandler, MyIdDirectoryNotFoundException>(true), Challenge);

    internal static void Challenge(BasicResult response)
    {
        response.Exception.ShouldNotBeNull();
        response.BadRequest.ShouldBeFalse();
        response.Forbidden.ShouldBeFalse();
        response.Unauthorized.ShouldBeFalse();
        response.PreconditionRequired.ShouldBeFalse();

        CustomLoggerMonitor.GetExceptionCount<MyIdDirectoryNotFoundHandler, MyIdDirectoryNotFoundException>()
            .ShouldBe(1);

    }

}

//- - - - - - - - - - - - - - - - - - //



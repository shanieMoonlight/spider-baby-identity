using ID.Domain.Utility.Exceptions;
using MediatR;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Mediatr.Pipeline.Exceptions.Helpers;

public class PingException(string? message) : MyIdException(message + " Thrown") { }

//- - - - - - - - - - - - - - - - - - //

public class PingHandler : IRequestHandler<TestExceptionsRequest, BasicResult>
{
    public Task<BasicResult> Handle(TestExceptionsRequest request, CancellationToken cancellationToken) =>
        throw new PingException(request.Message);
}

//- - - - - - - - - - - - - - - - - - //

public class PingExceptionTestHelper
{
    internal static TestParamaters Params =>
          new(() => MyContainerProvider.GetContainer<PingHandler, PingException>(true), Challenge);


    internal static void Challenge(BasicResult response)
    {
        response.Exception.ShouldNotBeNull();
        response.BadRequest.ShouldBeFalse();
        response.Forbidden.ShouldBeFalse();
        response.Unauthorized.ShouldBeFalse();
        response.PreconditionRequired.ShouldBeFalse();
    }

}

//- - - - - - - - - - - - - - - - - - //



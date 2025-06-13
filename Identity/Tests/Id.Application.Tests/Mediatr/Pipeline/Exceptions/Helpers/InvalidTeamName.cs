using ID.Domain.Utility.Exceptions;
using MediatR;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Mediatr.Pipeline.Exceptions.Helpers;

//- - - - - - - - - - - - - - - - - - //

public class InvalidTeamNameHandler : IRequestHandler<TestExceptionsRequest, BasicResult>
{
    public Task<BasicResult> Handle(TestExceptionsRequest request, CancellationToken cancellationToken) =>
        throw new InvalidTeamNameException(request.Message ?? "");
}

//- - - - - - - - - - - - - - - - - - //

public class InvalidTeamNameExceptionTestHelper
{
    internal static TestParamaters Params =>
          new(() => MyContainerProvider.GetContainer<InvalidTeamNameHandler, InvalidTeamNameException>(true), Challenge);


    internal static void Challenge(BasicResult response)
    {
        response.BadRequest.ShouldBeTrue();
    }

}

//- - - - - - - - - - - - - - - - - - //



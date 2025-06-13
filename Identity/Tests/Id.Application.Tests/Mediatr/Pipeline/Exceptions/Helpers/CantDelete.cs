using ID.Domain.Utility.Exceptions;
using MediatR;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Mediatr.Pipeline.Exceptions.Helpers;


//############################################//

public class CantDeleteHandler : IRequestHandler<TestExceptionsRequest, BasicResult>
{
    public Task<BasicResult> Handle(TestExceptionsRequest request, CancellationToken cancellationToken) =>
        throw new CantDeleteException(request.Message ?? "");
}

//############################################//

public class CantDeleteExceptionTestHelper
{
    internal static TestParamaters Params =>
          new(() => MyContainerProvider.GetContainer<CantDeleteHandler, CantDeleteException>(true), Challenge);


    internal static void Challenge(BasicResult response)
    {
        response.BadRequest.ShouldBeTrue();
    }

}

//############################################//
using ID.Domain.Utility.Exceptions;
using MediatR;
using MyResults;

namespace ID.Application.Tests.Mediatr.Pipeline.Exceptions.Helpers;

public class MySpecialException(string? message) : MyIdException(message + " Thrown") { }
public class NonMyIdException(string message) : Exception(message) { }

//- - - - - - - - - - - - - - - - - - //

public class MySpecialHandler : IRequestHandler<TestExceptionsRequest, BasicResult>
{
    public Task<BasicResult> Handle(TestExceptionsRequest request, CancellationToken cancellationToken) =>
        throw new MySpecialException(request.Message ?? "");
}

//- - - - - - - - - - - - - - - - - - //



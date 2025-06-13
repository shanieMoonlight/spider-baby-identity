using ID.Application.Mediatr.Behaviours.Exceptions;
using ID.Application.Tests.Mediatr.Pipeline.Exceptions.Helpers;
using ID.Domain.Utility.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using MyResults;

namespace ID.Application.Tests.Mediatr.Pipeline.Exceptions.Logger;


public class CustomLoggerMonitor
{
    public static int GetExceptionCount<THandler, TException>()
        where THandler : class, IRequestHandler<TestExceptionsRequest, BasicResult>
        where TException : MyIdException
    {

        var container = MyContainerProvider.GetContainer<MyIdFileNotFoundHandler, MyIdFileNotFoundException>(false);

        var logger = container.GetInstance<ILogger<IdRequestExceptionHandler<TestExceptionsRequest, BasicResult, TException>>>()
            as CustomLogger<IdRequestExceptionHandler<TestExceptionsRequest, BasicResult, TException>>;

        return logger?.GetExceptionCount<TException>() ?? 0;
    }
}


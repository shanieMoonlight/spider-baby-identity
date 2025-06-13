using ID.Application.Utility;
using ID.Domain.Utility.Exceptions;
using LoggingHelpers;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MyResults;

namespace ID.Application.Mediatr.Behaviours.Exceptions;

/// <summary>
/// Handles application exceptions and converts them to appropriate HTTP responses.
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type that must inherit from BasicResult</typeparam>
/// <typeparam name="TException">The exception type that must inherit from MyIdException</typeparam>
public class IdRequestExceptionHandler<TRequest, TResponse, TException>(
    ILogger<IdRequestExceptionHandler<TRequest, TResponse, TException>> logger)
    : IRequestExceptionHandler<TRequest, TResponse, TException>
      where TRequest : notnull
      where TResponse : BasicResult
      where TException : MyIdException
{

    public Task Handle(TRequest request, TException exception,
        RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        var response = exception switch
        {
            
            InvalidTeamNameException e => ResultFactory.CreateBadRequestResult<TResponse>(e.Message),
            DeviceLimitExceededException e => ResultFactory.CreateBadRequestResult<TResponse>(e.Message),
            CantDeleteException e => ResultFactory.CreateBadRequestResult<TResponse>(e.Message),
            MyIdDatabaseException e => ResultFactory.CreateBadRequestResult<TResponse>(e.Message),
            InvalidTwoFactorConfigurationException e => ResultFactory.CreateBadRequestResult<TResponse>(e.Message),

            //This is means that files or directories are missing so it must be an InternalServerError
            MyIdFileNotFoundException e => ResultFactory.CreateFailureResult<TResponse>(e),
            MyIdDirectoryNotFoundException e => ResultFactory.CreateFailureResult<TResponse>(e),
            _ => ResultFactory.CreateFailureResult<TResponse>(exception)
        };

        if (response is not null)
            state.SetHandled(response);

        //Unexpected so log it.
        if(response?.Status == BasicResult.ResultStatus.Failure)
            logger.LogException(exception, MyIdLoggingEvents.MEDIATR.UNKNOWN);

        return Task.CompletedTask;
    }



}//Cls

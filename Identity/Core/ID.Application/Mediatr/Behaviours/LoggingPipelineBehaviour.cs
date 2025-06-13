using MediatR;
using Microsoft.Extensions.Logging;
using MyResults;

namespace ID.Application.Mediatr.Behaviours;
public sealed class LoggingPipelineBehaviour<TRequest, TResponse>(ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : GenResult<TResponse>
{
    private readonly ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> _logger = logger;

    //-----------------------------//

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        LogStart();

        var result = await next(cancellationToken);


        LogEnd();

        return result;

    }

    //-----------------------------//

    private void LogStart() => BookendLog("Starting");

    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

    private void LogEnd() => BookendLog("Completed");

    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

    private void BookendLog(string type)
    {

        _logger.LogInformation("{type} request {RequestName}, {DateTimeUtc}", type, typeof(TRequest).Name, DateTime.UtcNow);

    }//LogInfo

    //-----------------------------//

    private void LogResult(TResponse result)
    {

        _logger.LogError(
            "Request {RequestName}, {Error}, {DateTimeUtc}",
            typeof(TRequest).Name,
            result.ToString(),
            DateTime.UtcNow);


    }//LogResult

    //-----------------------------//

}//Cls

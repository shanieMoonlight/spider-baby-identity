using MediatR;
using MyResults;

namespace ID.Application.Mediatr.Auth;
public abstract class AAuthGuardBehaviour<TRequest, TResponse, IAuthGuard>()
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : BasicResult
{
    //------------------------------------//

    protected abstract bool Authenticate();

    //------------------------------------//

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!IsCorrectType<IAuthGuard>(request))
            return await next(cancellationToken);

        return !Authenticate()
            ? CreateUnauthorizedResult()
            : await next(cancellationToken);
    }

    //------------------------------------//

    private TResponse CreateUnauthorizedResult(string? message = null)
    {

        if (typeof(TResponse) == typeof(BasicResult))
            return (BasicResult.UnauthorizedResult(message) as TResponse)!;

        var what = typeof(GenResult<>)
              .GetGenericTypeDefinition()
              .MakeGenericType(typeof(TResponse).GenericTypeArguments[0])
              .GetMethod(nameof(GenResult<object>.UnauthorizedResult), [typeof(string)])!
              ;

        //Must be GenResult
        return (TResponse)typeof(GenResult<>)
              .GetGenericTypeDefinition()
              .MakeGenericType(typeof(TResponse).GenericTypeArguments[0])
              .GetMethod(nameof(GenResult<object>.UnauthorizedResult), [typeof(string)])!
              .Invoke(null, [message])!
              ;

    }

    //------------------------------------//

    private static bool IsCorrectType<Interface>(TRequest request) =>
        request.GetType().GetInterfaces()
          .Any(x => x == typeof(Interface));


}//Cls
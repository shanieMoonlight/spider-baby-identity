using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Mediatr.Behaviours.Common;
internal class ResponseProvider
{

    internal static TResponse GenerateUnauthorizedUserResponse<TResponse>(string? userIdentifier = null) where TResponse : BasicResult
    {
        var msg = IDMsgs.Error.NotFound<AppUser>(userIdentifier ?? "User");

        if (typeof(TResponse) == typeof(BasicResult))
            return (BasicResult.UnauthorizedResult(msg) as TResponse)!;

        //Must be GenResult
        return (TResponse)typeof(GenResult<>)
        .GetGenericTypeDefinition()
              .MakeGenericType(typeof(TResponse).GenericTypeArguments[0])
              .GetMethod(nameof(GenResult<object>.UnauthorizedResult), [typeof(string)])!
              .Invoke(null, [msg])!;
    }

    //------------------------------------//

    internal static TResponse GenerateNotFoundResponse<TResponse, TNotFound>(string? notFoundItemIdentifier = null) where TResponse : BasicResult
    {
        var msg = IDMsgs.Error.NotFound<TNotFound>(notFoundItemIdentifier ?? "Team");

        if (typeof(TResponse) == typeof(BasicResult))
            return (BasicResult.NotFoundResult(msg) as TResponse)!;

        //Must be GenResult
        return (TResponse)typeof(GenResult<>)
        .GetGenericTypeDefinition()
              .MakeGenericType(typeof(TResponse).GenericTypeArguments[0])
              .GetMethod(nameof(GenResult<object>.NotFoundResult), [typeof(string)])!
              .Invoke(null, [msg])!;
    }

    //------------------------------------//

    internal static TResponse GenerateNotFoundResponse<TResponse>(string? msg = null) where TResponse : BasicResult
    {
        msg ??= "Resource Not Found ;[";

        if (typeof(TResponse) == typeof(BasicResult))
            return (BasicResult.NotFoundResult(msg) as TResponse)!;

        //Must be GenResult
        return (TResponse)typeof(GenResult<>)
        .GetGenericTypeDefinition()
              .MakeGenericType(typeof(TResponse).GenericTypeArguments[0])
              .GetMethod(nameof(GenResult<object>.NotFoundResult), [typeof(string)])!
              .Invoke(null, [msg])!;
    }

    //------------------------------------//

}//Cls

using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Mediatr.Behaviours.Exceptions;
internal static class ResultFactory
{
    private static readonly BasicResult _unknownBasicResult = BasicResult.Failure(IDMsgs.Error.UNKNOWN_ERROR);

    //---------------------------//

    /// <summary>
    /// Creates  a BasicResult/GenResult with the Exception attached. This will cause the controller to responde with a 500 status code
    /// </summary>
    public static TResult CreateFailureResult<TResult>(Exception ex) where TResult : BasicResult
    {
        try
        {
            if (typeof(TResult) == typeof(BasicResult))
                return (BasicResult.Failure(ex) as TResult)!;


            // Now create the proper concrete type
            var concreteGenResultType = GetConcreteGenResultType<TResult>();

            // Get method from the concrete type
            var method = concreteGenResultType
                .GetMethod(nameof(GenResult<object>.Failure), [typeof(Exception)])!;

            var response = (TResult)method.Invoke(null, [ex])!;


            return response is null
                ? (_unknownBasicResult as TResult)!
                : response;

        }
        catch (Exception)
        {
            return (_unknownBasicResult as TResult)!;
        }
    }


    //---------------------------//


    public static TResult CreateBadRequestResult<TResult>(string message) where TResult : BasicResult
    {
        try
        {
            if (typeof(TResult) == typeof(BasicResult))
                return (BasicResult.BadRequestResult(message) as TResult)!;

            // Now create the proper concrete type
            var concreteGenResultType = GetConcreteGenResultType<TResult>();

            // Get method from the concrete type
            var method = concreteGenResultType
                .GetMethod(nameof(GenResult<object>.BadRequestResult), [typeof(string)])!;

            var response = (TResult)method
                .Invoke(null, [message])!;

            return response is null
                ? (_unknownBasicResult as TResult)!
                : response;
        }
        catch (Exception)
        {
            return (_unknownBasicResult as TResult)!;
        }

    }

    //---------------------------//


    private static Type GetConcreteGenResultType<TResult>() where TResult : BasicResult
    {

        // First, extract the generic argument from TResult (assuming it's GenResult<T>)
        var genericArgType = typeof(TResult).GetGenericArguments()[0];

        // Now create the proper concrete type
        return typeof(GenResult<>).MakeGenericType(genericArgType);
    }



}//Cls
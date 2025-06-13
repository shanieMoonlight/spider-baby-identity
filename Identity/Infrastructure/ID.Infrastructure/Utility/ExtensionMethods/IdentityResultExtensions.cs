using Microsoft.AspNetCore.Identity;
using MyResults;

namespace ID.Infrastructure.Utility.ExtensionMethods;

public static class IdentityResultExtensions
{

    /// <summary>
    /// Converts IdentityResult to GenResult
    /// </summary>
    public static GenResult<T> ToGenResult<T>(this IdentityResult idResult, T value)
    {
        var info = string.Join(
                 ", " + Environment.NewLine,
                idResult.Errors.Select(err => "Code: " + err.Code + Environment.NewLine + err.Description)
            );

        return idResult.Succeeded
            ? GenResult<T>.Success(value, info)
            : GenResult<T>.Failure(info);

    }

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Converts IdentityResult to GenResult
    /// </summary>
    public static GenResult<T> ToGenResultFailure<T>(this IdentityResult idResult)
    {
        var info = string.Join(
                 ", " + Environment.NewLine,
                idResult.Errors.Select(err => "Code: " + err.Code + Environment.NewLine + err.Description)
            );

        return GenResult<T>.Failure(info);

    }

    //-----------------------------------//

    /// <summary>
    /// Converts IdentityResult to GenResult
    /// </summary>
    public static BasicResult ToBasicResult(this IdentityResult idResult, string? info = null)
    {
        string errorInfo = string.Join(
            ", " + Environment.NewLine,
            idResult.Errors.Select(err => "Code: " + err.Code + Environment.NewLine + err.Description)
        );

        string fullInfo = string.IsNullOrWhiteSpace(info)
            ? errorInfo
            : info + Environment.NewLine + errorInfo;

        return idResult.Succeeded
            ? BasicResult.Success(fullInfo)
            : BasicResult.BadRequestResult(fullInfo);
    }


}//Cls
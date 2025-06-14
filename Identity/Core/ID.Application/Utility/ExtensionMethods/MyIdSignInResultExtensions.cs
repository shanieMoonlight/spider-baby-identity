using ID.Application.AppAbs.SignIn;
using MyResults;

namespace ID.Application.Utility.ExtensionMethods;
public static class MyIdSignInResultExtensions
{

    public static GenResult<T> ToGenResult<T>(this MyIdSignInResult signInResult, T value)
    {
        if (signInResult.NotFound)
            return GenResult<T>.NotFoundResult(signInResult.Message);

        if (signInResult.Unauthorized)
            return GenResult<T>.UnauthorizedResult(signInResult.Message);

        if (signInResult.EmailConfirmationRequired)
            return GenResult<T>.PreconditionRequiredResult(value, signInResult.Message);

        if (signInResult.TwoFactorRequired)
            return GenResult<T>.PreconditionRequiredResult(value, signInResult.Message);

        if (!signInResult.Succeeded)
            return GenResult<T>.Failure(signInResult.Message);

        return GenResult<T>.Success(value, signInResult.Message);
    }


    //----------------------//

    public static BasicResult ToBasicResult(this MyIdSignInResult signInResult)
    {
        if (signInResult.NotFound)
            return BasicResult.NotFoundResult(signInResult.Message);

        if (signInResult.Unauthorized)
            return BasicResult.UnauthorizedResult(signInResult.Message);

        if (signInResult.EmailConfirmationRequired)
            return BasicResult.PreconditionRequiredResult(signInResult.Message);

        if (signInResult.TwoFactorRequired)
            return BasicResult.PreconditionRequiredResult(signInResult.Message);

        if (!signInResult.Succeeded)
            return BasicResult.Failure(signInResult.Message);

        return BasicResult.Success(signInResult.Message);
    }


}//Cls

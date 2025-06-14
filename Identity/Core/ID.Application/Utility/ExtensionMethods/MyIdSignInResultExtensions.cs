using ID.Application.AppAbs.SignIn;
using MyResults;

namespace ID.Application.Utility.ExtensionMethods;
public static class MyIdSignInResultExtensions
{

    public static GenResult<T> ToGenResult<T>(this MyIdSignInResult signInResult, T value)
    {
        //just in case signInResult doesn't consider EmailConfirmationRequired or TwoFactorRequired as failure. (It means the user WAS found)
        if (signInResult.EmailConfirmationRequired)
            return GenResult<T>.PreconditionRequiredResult(value, signInResult.Message);

        if (signInResult.TwoFactorRequired)
            return GenResult<T>.PreconditionRequiredResult(value, signInResult.Message);

        if (!signInResult.Succeeded)
            return signInResult.ToGenResultFailure<T>();

        return GenResult<T>.Success(value, signInResult.Message);
    }


    //----------------------//

    public static GenResult<T> ToGenResultFailure<T>(this MyIdSignInResult signInResult)
    {
        if (signInResult.EmailConfirmationRequired)
            return GenResult<T>.PreconditionRequiredResult(signInResult.Message);

        if (signInResult.TwoFactorRequired)
            return GenResult<T>.PreconditionRequiredResult(signInResult.Message);

        if (signInResult.NotFound)
            return GenResult<T>.NotFoundResult(signInResult.Message);

        if (signInResult.Unauthorized)
            return GenResult<T>.UnauthorizedResult(signInResult.Message);

        return GenResult<T>.Failure(signInResult.Message);

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

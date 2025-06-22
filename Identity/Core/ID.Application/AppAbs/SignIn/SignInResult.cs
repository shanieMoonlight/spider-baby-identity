using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;

namespace ID.Application.AppAbs.SignIn;
public class MyIdSignInResult
{
    public bool Succeeded { get; private set; } = false;
    public bool EmailConfirmationRequired { get; private set; } = false;
    public bool TwoFactorRequired { get; private set; } = false;
    public bool NotFound { get; private set; } = false;
    public bool Unauthorized { get; private set; } = false;
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Will be null if the user is not signed in.
    /// Succeeded=true => Team is not null
    /// </summary>
    public Team? Team { get; private set; }

    /// <summary>
    /// Will be null if the user is not signed in.
    /// Succeeded=true => User is not null
    /// </summary>
    public AppUser? User { get; private set; }

    public MfaResultData? MfaResultData { get; private set; }

    //------------------------------------//

    public static MyIdSignInResult Success(AppUser user, Team team) =>
        new()
        {
            Succeeded = true,
            User = user,
            Team = team,
            Message = "Signed In!!!"
        };


    //- - - - - - - - - - - - - - - - - - //

    public static MyIdSignInResult Failure(string reason) =>
        new()
        {
            Succeeded = false,
            Message = reason
        };


    //- - - - - - - - - - - - - - - - - - //

    public static MyIdSignInResult NotFoundResult() =>
        new()
        {
            Succeeded = false,
            NotFound = true,
            Message = IDMsgs.Error.Authorization.INVALID_LOGIN
        };


    //- - - - - - - - - - - - - - - - - - //

    public static MyIdSignInResult UnauthorizedResult() =>
        new()
        {
            Succeeded = false,
            Unauthorized = true,
            Message = IDMsgs.Error.Authorization.INVALID_LOGIN
        };


    //- - - - - - - - - - - - - - - - - - //

    public static MyIdSignInResult EmailConfirmedRequiredResult(string email) =>
        new()
        {
            Succeeded = false,
            EmailConfirmationRequired = true,
            Message = IDMsgs.Error.Email.EMAIL_NOT_CONFIRMED(email)
        };


    //- - - - - - - - - - - - - - - - - - //

    public static MyIdSignInResult TwoFactorRequiredResult(
        MfaResultData? mfaResultData,
        AppUser user,
        Team team) =>
        new()
        {
            Succeeded = false,
            User = user,
            Team = team,
            TwoFactorRequired = true,
            MfaResultData = mfaResultData,
            Message = IDMsgs.Error.Authorization.TWO_FACTOR_REQUIRED(mfaResultData?.TwoFactorProvider ?? user.TwoFactorProvider)
        };

    //------------------------------------//

}//Cls

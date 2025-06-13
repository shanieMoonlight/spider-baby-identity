using MyResults;
using StringHelpers;
using ID.Domain.Utility.Messages;
using ID.Domain.Models;
using ID.Domain.Entities.AppUsers;

namespace ID.Domain.Utility.Extensions;
public static class TwoFactorExtensions
{
    /// <summary>
    /// Can the <paramref name="user"/> enable TwoFactor Auth based off current configuration.
    /// I.e. if TwoFactorProvider is Sms the user must also have a PhoneNumber
    /// </summary>
    public static BasicResult CanEnableTwoFactor(this AppUser user) =>
        user.CanChangeToProvider(user.TwoFactorProvider);

    //- - - - - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Can the <paramref name="user"/> change to <paramref name="newProvider"/> based off current configuration.
    /// I.e. if  <paramref name="newProvider"/> is Sms the user must also have a PhoneNumber already set
    /// </summary>
    public static BasicResult CanChangeToProvider(this AppUser user, TwoFactorProvider newProvider)
    {
        if (newProvider == TwoFactorProvider.Sms && user.PhoneNumber.IsNullOrWhiteSpace())
            return BasicResult.Failure(IDMsgs.Error.TwoFactor.NO_PHONE_FOR_TWO_FACTOR(user));

        if (newProvider == TwoFactorProvider.Email && user.Email.IsNullOrWhiteSpace())
            return BasicResult.Failure(IDMsgs.Error.TwoFactor.NO_EMAIL_FOR_TWO_FACTOR(user));

        return BasicResult.Success();

    }

}//Cls

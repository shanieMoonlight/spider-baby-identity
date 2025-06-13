using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Infrastructure.TokenServices.Phone;

internal class DefaultPhoneConfirmationService<TUser>(IIdUserMgmtService<TUser> userMgr) : IPhoneConfirmationService<TUser> where TUser : AppUser
{
    public async Task<BasicResult> ConfirmPhoneAsync(Team team, TUser user, string token, string newPhone)
    {
        var succeeded = await userMgr.VerifyChangePhoneNumberTokenAsync(user, token, newPhone);

        return succeeded
           ? BasicResult.Success(IDMsgs.Info.Phone.PHONE_CONFIRMED(newPhone))
           : BasicResult.Failure(IDMsgs.Error.Phone.PHONE_CONFIRMATION_FAILURE(newPhone));
    }

    //-----------------------//

    public async Task<string> GeneratePhoneChangedConfirmationTokenAsync(Team team, TUser user, string newPhoneNumber) =>
      await userMgr.GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber);

    //-----------------------//

    public async Task<bool> IsPhoneConfirmedAsync(TUser user) =>
        await userMgr.IsPhoneNumberConfirmedAsync(user);

    //-----------------------//

}//Cls
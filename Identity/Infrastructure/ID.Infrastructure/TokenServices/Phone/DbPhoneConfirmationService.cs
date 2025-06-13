using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Setup.Options;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using Microsoft.Extensions.Options;
using MyResults;
using static ID.Domain.Utility.Messages.IDMsgs.Error;

namespace ID.Infrastructure.TokenServices.Phone;

internal class DbPhoneConfirmationService<TUser>(
    IIdUserMgmtService<TUser> userMgr,
    IIdentityTeamManager<TUser> _teamMgr,
    IIdUnitOfWork _uow,
    IOptions<IdGlobalOptions> _globalOptionsProvider)
    : IPhoneConfirmationService<TUser> where TUser : AppUser
{

    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;


    //-----------------------//

    public async Task<BasicResult> ConfirmPhoneAsync(Team team, TUser user, string confirmationToken, string newPhone)
    {

        if (IsTokenInvalid(user, confirmationToken, newPhone))
            return BasicResult.Failure(Tokens.InvalidTkn(nameof(IPhoneConfirmationService<TUser>)));

        await SetPhoneConfirmedAsync(team, user);
        return BasicResult.Success(IDMsgs.Info.Phone.PHONE_CONFIRMED(newPhone));
    }

    //-----------------------//

    public async Task<string> GeneratePhoneChangedConfirmationTokenAsync(Team team, TUser user, string newPhoneNumber)
    {
        var tkn = await userMgr.GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber);
        await AddTokenToUser(team, user, tkn);
        return tkn;
    }

    //-----------------------//

    private async Task SetPhoneConfirmedAsync(Team team, TUser user)
    {
        user.PhoneNumberConfirmed = true;
        user.SetTkn(null);
        await _teamMgr.UpdateMemberAsync(team, user);
        await _uow.SaveChangesAsync();
    }

    //-----------------------//

    private async Task AddTokenToUser(Team team, TUser user, string tkn)
    {
        user.SetTkn(tkn);
        await _teamMgr.UpdateMemberAsync(team, user);
        await _uow.SaveChangesAsync();
    }

    //-----------------------//

    public async Task<bool> IsPhoneConfirmedAsync(TUser user) =>
        await userMgr.IsPhoneNumberConfirmedAsync(user);

    //-----------------------//

    private bool IsTokenInvalid(AppUser user, string confirmationToken, string newPhone) =>
        user.Tkn != confirmationToken 
        || 
        user.PhoneNumber != newPhone
        || 
        user.TknModifiedDate is null
        || 
        user.TknModifiedDate.Value.Add(_globalOptions.PhoneTokenTimeSpan) < DateTime.Now;

}//Cls
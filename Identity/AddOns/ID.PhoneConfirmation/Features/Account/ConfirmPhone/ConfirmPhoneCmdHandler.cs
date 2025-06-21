using MyResults;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;

namespace ID.PhoneConfirmation.Features.Account.ConfirmPhone;
public class ConfirmPhoneCmdHandler(IPhoneConfirmationService<AppUser> _phoneConfService)
    : IIdCommandHandler<ConfirmPhoneCmd>
{

    public async Task<BasicResult> Handle(ConfirmPhoneCmd request, CancellationToken cancellationToken)
    {
        var token = request.Dto.ConfirmationToken;

        var user = request.PrincipalUser; //AUserAwareCommand ensures that this is not null
        var team = request.PrincipalTeam; //AUserAwareCommand ensures that this is not null

        if (await _phoneConfService.IsPhoneConfirmedAsync(user))
            return BasicResult.Success(IDMsgs.Info.Phone.PHONE_CONFIRMED(user.PhoneNumber ?? "no-phone"));

        var confirmationResult = await _phoneConfService.ConfirmPhoneAsync(team, user, token, user.PhoneNumber ?? "");

        return confirmationResult.Succeeded
            ? BasicResult.Success(IDMsgs.Info.Phone.PHONE_CONFIRMED(user.PhoneNumber ?? "no-phone"))
            : BasicResult.BadRequestResult(confirmationResult.Info);

    }


}//Cls

using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.PhoneConfirmation.Events.Integration.Bus;
using MyResults;

namespace ID.PhoneConfirmation.Features.Account.ResendPhoneConfirmationPrincipal;
public class ResendPhoneConfirmationPrincipalHandler(IPhoneConfirmationBus resendBus)
    : IIdCommandHandler<ResendPhoneConfirmationPrincipalCmd>
{

    public async Task<BasicResult> Handle(ResendPhoneConfirmationPrincipalCmd request, CancellationToken cancellationToken)
    {
        var user = request.PrincipalUser!;

        if (user.PhoneNumberConfirmed)
            return BasicResult.Success(IDMsgs.Info.Phone.PHONE_ALREADY_CONFIRMED(user.PhoneNumber));

        await resendBus.GenerateTokenAndPublishEventAsync(user, user.Team!, cancellationToken);


        return BasicResult.Success(IDMsgs.Info.Phone.PHONE_CONFIRMED(user.PhoneNumber));
    }


}//Cls

using ID.Application.AppAbs.ApplicationServices;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.Account.Cmd.ResendEmailConfirmationPrincipal;
public class ResendEmailConfirmationPrincipalHandler(IEmailConfirmationBus emailConfirmationBus)
    : IIdCommandHandler<ResendEmailConfirmationPrincipalCmd>
{
    public async Task<BasicResult> Handle(ResendEmailConfirmationPrincipalCmd request, CancellationToken cancellationToken)
    {
        var user = request.PrincipalUser!;

        if (user.EmailConfirmed)
            return BasicResult.Success(IDMsgs.Info.Email.EMAIL_ALREADY_CONFIRMED(user.Email));

        await emailConfirmationBus.GenerateTokenAndPublishEventAsync(user, user.Team!, cancellationToken);


        return BasicResult.Success(IDMsgs.Info.Email.EMAIL_CONFIRMATION_SENT(user.Email));
    }


}//Cls

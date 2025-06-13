using ID.Application.AppAbs.ApplicationServices;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.Account.Cmd.ResendEmailConfirmation;
public class ResendEmailConfirmationHandler(IFindUserService<AppUser> findUserService, IEmailConfirmationBus resendEmailService) : IIdCommandHandler<ResendEmailConfirmationCmd>
{

    public async Task<BasicResult> Handle(ResendEmailConfirmationCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        //Can't us AUserAwareCommand as the team leader or Maintenance member might need to trigger this for a new user who didn't receive the Reg Emails
        var user = await findUserService.FindUserWithTeamDetailsAsync(dto.Email, dto.Username, dto.UserId);
        if (user == null)
            return BasicResult.NotFoundResult(IDMsgs.Error.Authorization.INVALID_AUTH);

        if (user.EmailConfirmed)
            return BasicResult.Success(IDMsgs.Info.Email.EMAIL_ALREADY_CONFIRMED(user.Email));

        await resendEmailService.GenerateTokenAndPublishEventAsync(user, user.Team!, cancellationToken);


        return BasicResult.Success(IDMsgs.Info.Email.EMAIL_CONFIRMATION_SENT(user.Email));

    }

}//Cls

using MyResults;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.ConfirmEmail;
public class ConfirmEmailCmdHandler(IFindUserService<AppUser> findUserService, IEmailConfirmationService<AppUser> _emailConfService)
    : IIdCommandHandler<ConfirmEmailCmd>
{

    public async Task<BasicResult> Handle(ConfirmEmailCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        //Check if user exists
        var user = await findUserService.FindUserWithTeamDetailsAsync(dto.UserId);
        if (user == null)
            return BasicResult.NotFoundResult(IDMsgs.Error.Authorization.INVALID_AUTH);


        if (user.EmailConfirmed)
            return BasicResult.Success(IDMsgs.Info.Email.EMAIL_ALREADY_CONFIRMED(user.Email));


        var confirmationResult = await _emailConfService.ConfirmEmailAsync(user.Team!, user, dto.ConfirmationToken);
        return confirmationResult.Succeeded
            ? BasicResult.Success(IDMsgs.Info.Email.EMAIL_CONFIRMED)
            : BasicResult.BadRequestResult(confirmationResult.Info);
    }

}//Cls

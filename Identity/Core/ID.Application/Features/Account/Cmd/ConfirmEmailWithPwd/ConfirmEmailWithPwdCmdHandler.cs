using MyResults;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.ConfirmEmailWithPwd;
public class ConfirmEmailWithPwdCmdHandler(IFindUserService<AppUser> findUserService, IEmailConfirmationService<AppUser> _emailConfService)
    : IIdCommandHandler<ConfirmEmailWithPwdCmd>
{

    public async Task<BasicResult> Handle(ConfirmEmailWithPwdCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        //Check if user exists
        var user = await findUserService.FindUserWithTeamDetailsAsync(dto.UserId);
        if (user == null)
            return BasicResult.NotFoundResult(IDMsgs.Error.Authorization.INVALID_AUTH);


        if (user.EmailConfirmed)
            return BasicResult.Success(IDMsgs.Info.Email.EMAIL_ALREADY_CONFIRMED(user.Email));


        var confirmationResult = await _emailConfService.ConfirmEmailWithPasswordAsync(user.Team!, user, dto.ConfirmationToken, dto.Password);

        return confirmationResult.Succeeded
            ? BasicResult.Success(IDMsgs.Info.Email.EMAIL_CONFIRMED)
            : BasicResult.BadRequestResult(confirmationResult.Info);
    }

}//Cls

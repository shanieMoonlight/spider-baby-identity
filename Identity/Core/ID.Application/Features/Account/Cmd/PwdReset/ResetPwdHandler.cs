using MyResults;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.PwdReset;
public class ResetPwdHandler(IFindUserService<AppUser> findUserService, IPwdResetService<AppUser> _pwdReset) : IIdCommandHandler<ResetPwdCmd>
{

    public async Task<BasicResult> Handle(ResetPwdCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        //Check if user exists
        var user = await findUserService.FindUserWithTeamDetailsAsync(dto.Email, dto.Username, dto.UserId);
        if (user == null)
            return BasicResult.NotFoundResult(IDMsgs.Error.Authorization.INVALID_AUTH);


        var resetResult =   await _pwdReset.ResetPasswordAsync(user.Team!, user, dto.ResetToken, dto.NewPassword, cancellationToken);

        return !resetResult.Succeeded
            ? BasicResult.BadRequestResult(resetResult.Info)
            : BasicResult.Success(IDMsgs.Info.Passwords.PASSWORD_CHANGE_SUCCESSFUL);
    }

}//Cls

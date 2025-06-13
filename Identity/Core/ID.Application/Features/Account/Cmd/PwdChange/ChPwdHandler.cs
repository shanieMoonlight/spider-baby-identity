using MyResults;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.PwdChange;
public class ChPwdHandler(IIdUserMgmtService<AppUser> userMgr)
    : IIdCommandHandler<ChPwdCmd>
{
    public async Task<BasicResult> Handle(ChPwdCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var user = request.PrincipalUser!;

        bool validPwd = await userMgr.CheckPasswordAsync(user, dto.Password);
        if (!validPwd)
            return BasicResult.UnauthorizedResult(IDMsgs.Error.Authorization.INVALID_AUTH);

        var passwordChangeResult =  await userMgr.ChangePasswordAsync(user, dto.Password ?? "", dto.NewPassword ?? "");

        if (!passwordChangeResult.Succeeded)
            return passwordChangeResult;

        return BasicResult.Success(IDMsgs.Info.Passwords.PASSWORD_CHANGE_SUCCESSFUL);

    }

}//Cls


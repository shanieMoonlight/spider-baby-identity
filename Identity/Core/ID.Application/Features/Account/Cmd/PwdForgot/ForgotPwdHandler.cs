using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.ForgotPwd;
using MyResults;
using StringHelpers;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.PwdForgot;


public class ForgotPwdHandler(IFindUserService<AppUser> findUserService, IPwdResetService<AppUser> _pwdReset, IEventBus bus)
    : IIdCommandHandler<ForgotPwdCmd>
{

    public async Task<BasicResult> Handle(ForgotPwdCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        //Check if user exists
        var user = await findUserService.FindUserWithTeamDetailsAsync(dto.Email, dto.Username, dto.UserId);
        if (user == null)
            return BasicResult.NotFoundResult(IDMsgs.Error.Authorization.INVALID_AUTH);

        if (user.Email.IsNullOrWhiteSpace())
            return BasicResult.NotFoundResult(IDMsgs.Error.Email.USER_WITHOUT_EMAIL);


        string safePwdResetTkn = await _pwdReset.GenerateSafePasswordResetTokenAsync(user.Team!, user);
        await bus.Publish(
            new ForgotPwdEmailRequestIntegrationEvent(
                user,
                safePwdResetTkn,
                user.Team!.TeamType == TeamType.customer),
            cancellationToken);

        //We must trust Publish
        return BasicResult.Success(IDMsgs.Info.Passwords.PASSWORD_RESET);
    }

}//Cls

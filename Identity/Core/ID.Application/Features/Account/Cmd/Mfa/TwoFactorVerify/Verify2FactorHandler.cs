using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.JWT;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Models;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorVerify;
public class Verify2FactorHandler(
    IJwtPackageProvider _jwtPackageProvider,
    IFindUserService<AppUser> _findUserService,
    ITwoFactorVerificationService<AppUser> _2FactorService)
    : IIdCommandHandler<Verify2FactorCmd, JwtPackage>
{

    public async Task<GenResult<JwtPackage>> Handle(Verify2FactorCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var userId = request.PrincipalUserId ?? dto.UserId; //Two factor failure may have returned a Jwt or Cookie. If not clien cust supply an ID
        var user = await _findUserService.FindUserWithTeamDetailsAsync(userId: userId);
        var team = user?.Team; 


        if (user is null || team is null)
            return GenResult<JwtPackage>.UnauthorizedResult(IDMsgs.Error.Authorization.INVALID_AUTH);


        bool validVerification = await _2FactorService.VerifyTwoFactorTokenAsync(team, user, dto.Token);
        if (!validVerification)
            return GenResult<JwtPackage>.BadRequestResult(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_TOKEN);

        //Package all user info in JWT and send it back to client.
        JwtPackage jwtPackage = await _jwtPackageProvider.CreateJwtPackageAsync(
            user: user,
            team: user.Team!,
            twoFactorVerified: true,
            currentDeviceId: dto.DeviceId,
            cancellationToken: cancellationToken);

        return GenResult<JwtPackage>.Success(jwtPackage);
    }


}//Cls

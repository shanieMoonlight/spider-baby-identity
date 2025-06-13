using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Application.Mediatr.CqrsAbs;
using MyResults;

namespace ID.Application.Features.Account.Qry.GetTwoFactorAppSetupData;
public class GetTwoFactorAppSetupDataQryHandler(IAuthenticatorAppService authAppService)
    : IIdQueryHandler<GetTwoFactorAppSetupDataQry, AuthAppSetupDto>
{

    public async Task<GenResult<AuthAppSetupDto>> Handle(GetTwoFactorAppSetupDataQry request, CancellationToken cancellationToken)
    {
        var user = request.PrincipalUser!; // UserAwarePipelineBehavior ensures this is not null

        var setupDto = await authAppService.Setup(user);

        return GenResult<AuthAppSetupDto>.Success(setupDto);

    }

}//Cls

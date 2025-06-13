using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Application.Mediatr.Cqrslmps.Queries;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Qry.GetTwoFactorAppSetupData;
public record GetTwoFactorAppSetupDataQry() : AIdUserAwareQuery<AppUser, AuthAppSetupDto>;




using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorResend;
public record Resend2FactorCmd(Resend2FactorDto Dto) : AIdUserAndTeamAwareCommand<AppUser, MfaResultData>;




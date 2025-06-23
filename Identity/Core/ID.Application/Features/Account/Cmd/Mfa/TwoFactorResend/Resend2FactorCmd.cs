using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorResend;
public record Resend2FactorCmd(Resend2FactorDto Dto) : AIdCommand<MfaResultData>;




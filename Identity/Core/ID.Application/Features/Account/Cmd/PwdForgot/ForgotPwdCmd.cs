using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.Account.Cmd.PwdForgot;
public record ForgotPwdCmd(ForgotPwdDto Dto) : AIdCommand;




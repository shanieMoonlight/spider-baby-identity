using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.Account.Cmd.PwdReset;
public record ResetPwdCmd(ResetPwdDto Dto) : AIdCommand;




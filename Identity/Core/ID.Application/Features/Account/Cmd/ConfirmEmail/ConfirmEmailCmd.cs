using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.Account.Cmd.ConfirmEmail;
public record ConfirmEmailCmd(ConfirmEmailDto Dto) : AIdCommand;




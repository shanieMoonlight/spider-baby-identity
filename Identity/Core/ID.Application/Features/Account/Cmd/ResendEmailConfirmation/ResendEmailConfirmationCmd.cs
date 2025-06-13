using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.Account.Cmd.ResendEmailConfirmation;
public record ResendEmailConfirmationCmd(ResendEmailConfirmationDto Dto) : AIdCommand;




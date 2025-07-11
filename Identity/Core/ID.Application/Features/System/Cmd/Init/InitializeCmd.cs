using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.System.Cmd.Init;

public record InitializeDto(string Password, string? Email);
public record InitializeCmd(InitializeDto Dto) : AIdCommand;




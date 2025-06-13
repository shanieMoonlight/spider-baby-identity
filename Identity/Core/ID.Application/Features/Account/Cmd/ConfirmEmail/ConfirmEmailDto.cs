namespace ID.Application.Features.Account.Cmd.ConfirmEmail;

public record ConfirmEmailDto(Guid? UserId, string ConfirmationToken);
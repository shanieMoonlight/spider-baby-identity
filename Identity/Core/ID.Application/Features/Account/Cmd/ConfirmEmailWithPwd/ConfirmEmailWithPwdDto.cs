namespace ID.Application.Features.Account.Cmd.ConfirmEmailWithPwd;

public record ConfirmEmailWithPwdDto(
    Guid? UserId,
    string ConfirmationToken,
    string Password,
    string ConfirmPassword);
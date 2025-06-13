namespace ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppComplete;
public record TwoFactorAuthAppCompleteRegDto(
        string TwoFactorCode,
        string CustomerSecretKey
    );





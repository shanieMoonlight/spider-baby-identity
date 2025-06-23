namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorResend;

public class Resend2FactorDto
{
    public Guid? UserId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string Token { get; set; } = string.Empty;

}//Cls
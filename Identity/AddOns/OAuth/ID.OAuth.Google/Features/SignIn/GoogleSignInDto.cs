namespace ID.OAuth.Google.Features.SignIn;

public class GoogleSignInDto
{
    public string IdToken { get; set; } = string.Empty;

    public Guid? SubscriptionId { get; set; }

    public string? DeviceId { get; set; }


}
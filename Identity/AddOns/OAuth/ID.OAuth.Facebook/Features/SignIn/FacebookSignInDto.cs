namespace ID.OAuth.Facebook.Features.SignIn;

public class FacebookSignInDto
{
    public string IdToken { get; set; } = string.Empty;

    public Guid? SubscriptionId { get; set; }

    public string? DeviceId { get; set; }


}
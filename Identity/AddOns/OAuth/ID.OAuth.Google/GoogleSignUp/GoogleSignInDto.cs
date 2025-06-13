namespace ID.OAuth.Google.GoogleSignUp;

public sealed class GoogleSignInDto
{
    public string IdToken { get; set; } = string.Empty;

    public Guid? SubscriptionId { get; set; }

    public string? DeviceId { get; set; }


}
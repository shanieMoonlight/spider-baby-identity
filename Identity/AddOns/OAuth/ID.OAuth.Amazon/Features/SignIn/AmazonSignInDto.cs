namespace ID.OAuth.Amazon.Features.SignIn;

public class AmazonSignInDto
{
    /// <summary>
    /// Amazon access token (LWA)
    /// </summary>
    public string AuthToken { get; set; } = string.Empty;

    /// <summary>
    /// Amazon User ID (optional, used for consistency checks)
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Email fallback if profile email is missing
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Id of the subscription plan to assign to the user (optional)
    /// </summary>
    public Guid? SubscriptionPlanId { get; set; }

    /// <summary>
    /// ID of Device used for sign-in (optional)
    /// </summary>
    public string? DeviceId { get; set; }
}

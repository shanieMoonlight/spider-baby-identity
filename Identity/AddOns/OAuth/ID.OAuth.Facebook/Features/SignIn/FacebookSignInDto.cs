namespace ID.OAuth.Facebook.Features.SignIn;

public class FacebookSignInDto
{
    /// <summary>
    /// Facebook temporary authentication token
    /// </summary>
    public string AuthToken { get; set; } = string.Empty;

    /// <summary>
    /// Facebook User ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Email in Facebook profile (optional, used if Facebook profile email is missing)
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

}//Cls
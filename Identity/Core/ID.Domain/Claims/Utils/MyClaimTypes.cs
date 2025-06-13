using ID.GlobalSettings.Setup.Settings;


namespace ID.Domain.Claims.Utils;

public class MyIdClaimTypes
{

    public static readonly string CLAIM_TYPE_PREFIX = IdGlobalSettings.ClaimTypePrefix;

    /// <summary>
    /// Claim describing the the app being used to contact server.
    /// </summary>
    public static readonly string APPLICATION_ID = $"{CLAIM_TYPE_PREFIX}.ApplicationId";

    /// <summary>
    /// Claim describing the the company that the user works for.
    /// </summary>
    public static readonly string COMPANY = $"{CLAIM_TYPE_PREFIX}.Company";

    /// <summary>
    /// Claim describing the the service that is contacting the server.
    /// </summary>
    public static readonly string REQUESTING_SERVICE = $"{CLAIM_TYPE_PREFIX}.requestingService";

    /// <summary>
    /// Claim describing the Role of user. (Manager, Employee, Customer etc.)
    /// </summary>
    public static readonly string ROLE = $"{CLAIM_TYPE_PREFIX}.role";

    /// <summary>
    /// Claim describing the Name of user.
    /// </summary>
    public static readonly string NAME = $"name";

    /// <summary>
    /// List of registered devices
    /// </summary>
    public static readonly string DEVICES = $"{CLAIM_TYPE_PREFIX}.devices.deviceList";

    /// <summary>
    /// List of apps that this user has connected with their account.
    /// </summary>
    public static readonly string APPS = $"{CLAIM_TYPE_PREFIX}.apps.AppList";

    /// <summary>
    /// The name of the application being used 
    /// </summary>
    public static readonly string APPLICATION = $"{CLAIM_TYPE_PREFIX}.apps.ApplicationName";

    /// <summary>
    /// List of registered devices
    /// </summary>
    public static readonly string CURRENT_DEVICE = $"{CLAIM_TYPE_PREFIX}.devices.CurrentDeviceId";

    /// <summary>
    /// First Name of user (Optional)
    /// </summary>
    public static readonly string FIRST_NAME = $"firstName";

    /// <summary>
    /// Last Name of user (Optional)
    /// </summary>
    public static readonly string LAST_NAME = $"lastName";

    /// <summary>
    /// Last Name of user (Optional)
    /// </summary>
    public static readonly string EMAIL = $"email";

    /// <summary>
    /// User has logged in using two-factor authentication
    /// </summary>
    public static readonly string TWO_FACTOR_VERIFIED = $"{CLAIM_TYPE_PREFIX}.two_factor_verified";

    /// <summary>
    /// User has logged in using but still needs two-factor authentication
    /// </summary>
    public static readonly string TWO_FACTOR_REQUIRED = $"{CLAIM_TYPE_PREFIX}.two_factor_required";

    /// <summary>
    /// Claim describing the ID of the team.
    /// </summary>
    public static readonly string TEAM_ID = $"{CLAIM_TYPE_PREFIX}.team_id";

    /// <summary>
    /// Claim describing the type of the team.
    /// </summary>
    public static readonly string TEAM_TYPE = $"{CLAIM_TYPE_PREFIX}.team_type";

    /// <summary>
    /// Claim describing the position of the user in the team.
    /// </summary>
    public static readonly string TEAM_POSITION = $"{CLAIM_TYPE_PREFIX}.team_position";

    /// <summary>
    /// Claim describing the subscriptions of the team.
    /// </summary>
    public static readonly string TEAM_SUBSCRIPTIONS = $"{CLAIM_TYPE_PREFIX}.subscriptions";

    /// <summary>
    /// Claim describing subscription with name <paramref name="name"/>.
    /// </summary>
    public static string TEAM_SUBSCRIPTION(string name, string value) => $"{CLAIM_TYPE_PREFIX}.subscriptions.{name}.{value}";


}//Cls

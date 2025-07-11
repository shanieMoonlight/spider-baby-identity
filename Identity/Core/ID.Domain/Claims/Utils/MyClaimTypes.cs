using ID.GlobalSettings.Setup.Settings;


namespace ID.Domain.Claims.Utils;

public class MyIdClaimTypes
{

    public static readonly string CLAIM_TYPE_PREFIX = IdGlobalSettings.ClaimTypePrefix;

    /// <summary>
    /// Claim describing the the app being used to contact server.
    /// </summary>
    public static readonly string APPLICATION_ID = $"{CLAIM_TYPE_PREFIX}.app_id";

    /// <summary>
    /// Claim describing the the company that the user works for.
    /// </summary>
    public static readonly string COMPANY = $"{CLAIM_TYPE_PREFIX}.company";


    /// <summary>
    /// Claim describing the Role of user. (Manager, Employee, Customer etc.)
    /// </summary>
    public static readonly string ROLE = $"role";

    /// <summary>
    /// Claim describing the Name of user.
    /// </summary>
    public static readonly string NAME = $"name";


    /// <summary>
    /// The name of the application being used 
    /// </summary>
    public static readonly string APPLICATION = $"{CLAIM_TYPE_PREFIX}.apps.application";

    /// <summary>
    /// List of registered devices
    /// </summary>
    public static readonly string CURRENT_DEVICE = $"{CLAIM_TYPE_PREFIX}.devices.current_device_id";

    /// <summary>
    /// Max Devices allowed for the subscription
    /// </summary>
    public static readonly string DEVICE_LIMIT = $"{CLAIM_TYPE_PREFIX}.devices.device_limit";

    /// <summary>
    /// First Name of user (Optional)
    /// </summary>
    public static readonly string FIRST_NAME = $"given_name";

    /// <summary>
    /// Last Name of user (Optional)
    /// </summary>
    public static readonly string LAST_NAME = $"family_name";

    /// <summary>
    /// Last Name of user (Optional)
    /// </summary>
    public static readonly string EMAIL = $"email";
    /// <summary>
    /// Time zone information for the user (Optional)
    /// </summary>
    public static readonly string ZONEINFO = "zoneinfo";

    /// <summary>
    /// The time the user's information was last updated (Optional)
    /// </summary>
    public static readonly string UPDATED_AT = "updated_at";

    /// <summary>
    /// The user's birthdate (Optional)
    /// </summary>
    public static readonly string BIRTHDATE = "birthdate";

    /// <summary>
    /// The user's gender (Optional)
    /// </summary>
    public static readonly string GENDER = "gender";

    /// <summary>
    /// The user's address (Optional)
    /// </summary>
    public static readonly string ADDRESS = "address";

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

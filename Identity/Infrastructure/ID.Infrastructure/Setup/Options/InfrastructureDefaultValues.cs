namespace ID.Infrastructure.Setup.Options;

public class InfrastructureDefaultValues
{
    /// <summary>
    /// false
    /// </summary>
    public const bool USE_DB_TOKEN_PROVIDER = false;

    /// <summary>
    /// true
    /// </summary>
    public const bool ALLOW_EXTERNAL_PAGES_DEV_MODE_ACCESS = true;


    // "/swagger"
    /// <summary>
    /// "/swagger";
    /// </summary>
    public const string SWAGGER_URL = "/swagger";

    /// <summary>
    /// 15
    /// </summary>
    internal static readonly int MAX_TRUSTED_DEVICES_PER_USER = 15;
}//Cls
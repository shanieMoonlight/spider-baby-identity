// IdGlobalConstants.Authentication.cs
namespace ID.GlobalSettings.Constants;

internal partial class IdGlobalConstants
{
    internal static partial class TrustedDevices
    {
        // Number of days after expiry when we must delete trusted devices
        /// <summary>
        /// "Authorization"
        /// </summary>
        internal const int MAX_EXPIRED_BY_DAYS = 30;
    }
}

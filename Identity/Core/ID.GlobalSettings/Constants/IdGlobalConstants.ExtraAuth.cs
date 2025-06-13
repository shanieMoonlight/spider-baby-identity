// IdGlobalConstants.ExtraAuth.cs
namespace ID.GlobalSettings.Constants;

internal partial class IdGlobalConstants
{
    internal static partial class ExtraAuth
    {
        /// <summary>
        /// The key to look for in the query params for the JWT token.
        /// Used to Authenticate for HangFire and Swagger. In a jwt only app.
        /// </summary>
        internal const string JWT_QUERY_KEY = "tkn";

        /// <summary>
        /// The duration in hours for which the authentication cookie is valid.
        /// </summary>
        internal const int COOKIE_DURATION_HOURS = 1;
    }
}

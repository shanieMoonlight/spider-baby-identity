// IdGlobalConstants.Authentication.cs
namespace ID.GlobalSettings.Constants;

internal partial class IdGlobalConstants
{
    internal static partial class Authentication
    {
        // Name of bearer token header
        /// <summary>
        /// "Authorization"
        /// </summary>
        internal const string TOKEN_HEADER_KEY = "Authorization";

        // The name of the cookie scheme being used.
        /// <summary>
        /// "shanie.moonlight.myidentity.Cookie.AuthenticationScheme"
        /// </summary>
        internal const string COOKIE_AUTHENTICATION_SCHEME = "spiderbaby.myid.Cookie.AuthenticationScheme";

        // Name of cookie being produced
        /// <summary>
        /// "shanie.moonlight.myidentity.Cookie"
        /// </summary>
        internal const string COOKIE_NAME = "spiderbaby.myid.Cookie";

        // Whether cookie is stored in browser after logging off. 
        //True since it has a one hour timeout
        /// <summary>
        /// true
        /// </summary>
        internal const bool COOKIE_IS_PERSISTENT = true;
    }
}

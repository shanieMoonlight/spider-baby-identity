namespace ID.GlobalSettings.Routes;



/// <summary>
/// This contains the names of routes.
/// Use when you need to access route names in Middleware etc.
///  <para>
///  Controller.Action: 
///  [HttpPost($"{Routes.Account.Actions.TwoFactorVerification}")]
/// </para>
/// <para>
/// Middleware:
/// request.RouteValues["controller"].Equals(Routes.Account.Controller, StringComparison.CurrentCultureIgnoreCase)
/// </para>
/// </summary>
public static class IdRoutes
{
    public const string Base = "identity";


    //+ + + + + + + + + + + + + + + + + + //

    public static class Account
    {
        public const string Controller = "Account";

        public static class Actions
        {
            public const string TwoFactorVerification = "TwoFactorVerification";
            public const string TwoFactorVerificationCookie = "TwoFactorVerificationCookie";
            public const string TwoFactorResend = "ResendTwoFactor";
            public const string TwoFactorResendCookie = "ResendTwoFactorCookie";
            public const string Login = "Login";
            public const string CookieSignIn = "CookieSignIn";
        }
    }

    //+ + + + + + + + + + + + + + + + + + //

    public static class UserManagement
    {
        public const string Controller = "UserManagement";

        public static class Actions
        {
            public const string UpdatePosition = "UpdatePosition";
        }
    }

    //+ + + + + + + + + + + + + + + + + + //

}//Cls

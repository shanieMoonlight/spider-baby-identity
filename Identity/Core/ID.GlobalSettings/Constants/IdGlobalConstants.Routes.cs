// IdGlobalConstants.EmailRoutes.cs
namespace ID.GlobalSettings.Constants;

internal partial class IdGlobalConstants
{
    internal static partial class EmailRoutes
    {
        internal const string ConfirmEmail = "confirm-email";
        internal const string ConfirmEmailWithPassword = "confirm-email-with-password";
        internal const string ResetPassword = "resetpassword";

        internal static partial class Params
        {
            internal const string UserId = "userid";
            internal const string ConfirmationToken = "confirmationtoken";
        }
    }

    internal static class PhoneRoutes
    {
        internal const string ConfirmPhone = "confirm-phone";

        internal static class Params
        {
            internal const string UserId = "userid";
            internal const string ConfirmationToken = "confirmationtoken";
        }

    }//Cls
}

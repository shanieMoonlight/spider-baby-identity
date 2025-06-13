using ID.Domain.Entities.AppUsers;

namespace ID.Domain.Utility.Messages;

public partial class IDMsgs
{      

    public static class Info
    {
        public static string INITIALIZED(string email) => $"Super team setup is complete with team leader email '{email}'. " +
            $"Full details : /identity/account/myinfo. " +
            $"Update details : /identity/usermanagement/update/member. " +
            $"This endpoint will return 404-NotFound from now on.";
        public static string Deleted<T>(object identifier) => $"{typeof(T).Name}, {identifier}, has been deleted";
        public static string USER_UPDATED(string username) => $"User, {username} has been updated";

        //---------------------------//


        public static class Passwords
        {
            public const string PASSWORD_CHANGE_SUCCESSFUL = "Your password has been changed.";
            public const string PASSWORD_RESET = "A password reset message was sent to your email address. Make sure to check your Spam folder if it's not in your inbox in a few minutes";
        }


        //---------------------------//

        public static class Email
        {
            public const string EMAIL_CONFIRMED = "Email has been confirmed!";
            public static string EMAIL_ALREADY_CONFIRMED(string? email) => $"Email {email} has already been confirmed. Try logging in.";
            public static string EMAIL_CONFIRMATION_SENT(string? email) => $"A confirmation email sent to {email ?? "no-email"}. Make sure to check the Spam Folder if it's not in the inbox in a few minutes";
        }

        //---------------------------//

        public static class Phone
        {
            public static string PHONE_ALREADY_CONFIRMED(string? phone) => $"Phone {phone ?? "no-phone"} has already been confirmed.";
            public static string PHONE_CONFIRMED(string? phone) => $"Phone, {phone ?? "no-phone"} number has been confirmed!";
        }

        //---------------------------//

        public static class Tokens
        {
            public static string TokensRemovedForUser(AppUser user) => $"All token for {user.FriendlyName} removed!";

        }

        //---------------------------//
    }


}//Cls


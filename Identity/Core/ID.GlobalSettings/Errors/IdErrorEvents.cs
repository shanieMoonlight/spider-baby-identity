namespace ID.GlobalSettings.Errors;
public class IdErrorEvents
{
    public const int OutboxProcessing = 38701;

    public static class Email
    {
        public const int ForgotPassword = 38901;
        public const int ResetPassword = ForgotPassword + 1;
        public const int EmailConfirmation = ResetPassword + 1;
        public const int TwoFactor = EmailConfirmation + 1;
        public const int EmailSetup = TwoFactor + 1;
        public const int PhoneConfirmation = EmailSetup + 1;
    }

    public static class Listeners
    {
        public const int Unknown = 39901;
        public const int TeamMemberCreated = Unknown + 1;
        public const int UserEmailUpdated = TeamMemberCreated + 1;
        public const int TwoFactorAuthSetup = UserEmailUpdated + 1;
        public const int UserPhoneUpdated = TwoFactorAuthSetup + 1;
        public const int TwoFactorUpdated = UserPhoneUpdated + 1;
        public const int TeamPositionRangeUpdated = TwoFactorUpdated + 1;
        public const int TeamSubscriptionDeactivated = TeamPositionRangeUpdated + 1;
    }
    public static class OAuth
    {
        public const int Verification = 40001;
    }

}

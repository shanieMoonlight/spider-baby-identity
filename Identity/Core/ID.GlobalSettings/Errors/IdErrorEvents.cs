namespace ID.GlobalSettings.Errors;
public class IdErrorEvents
{

    public const int Unexpected = 666;
    public const int Startup = Unexpected + 1;

    public static class DB
    {
        public const int InvalidProperty = 10001;
    }

    public static class Email
    {
        public const int ForgotPassword = 11001;
        public const int ResetPassword = ForgotPassword + 1;
        public const int EmailConfirmation = ResetPassword + 1;
        public const int TwoFactor = EmailConfirmation + 1;
        public const int EmailSetup = TwoFactor + 1;
        public const int PhoneConfirmation = EmailSetup + 1;
        public const int TrustedDevices = PhoneConfirmation + 1;
    }

    public static class Jobs
    {
        public const int OutboxProcessing = 12001;
        public const int OldOutboxProcessing = OutboxProcessing + 1;
        public const int DbMntc = OldOutboxProcessing + 1;
    }


    public static class Mediatr
    {
        public const int Unexpected = 13001;
    }



    public static class Listeners
    {
        public const int Unknown = 14001;
        public const int TeamMemberCreated = Unknown + 1;
        public const int UserEmailUpdated = TeamMemberCreated + 1;
        public const int TwoFactorAuthSetup = UserEmailUpdated + 1;
        public const int UserPhoneUpdated = TwoFactorAuthSetup + 1;
        public const int TwoFactorUpdated = UserPhoneUpdated + 1;
        public const int TeamPositionRangeUpdated = TwoFactorUpdated + 1;
        public const int TeamSubscriptionDeactivated = TeamPositionRangeUpdated + 1;
        public const int TrustedDeviceAdded = TeamSubscriptionDeactivated + 1;
        public const int TrustedDeviceExtended = TrustedDeviceAdded + 1;
        public const int TrustedDeviceRevoked = TrustedDeviceExtended + 1;
        public const int TrustedDeviceUsed = TrustedDeviceRevoked + 1;
    }
    public static class OAuth
    {
        public const int Verification = 15001;
        public const int Facebook = Verification + 1;
        public const int Google = Facebook + 1;
        public const int Amazon = Google + 1;
    }

}

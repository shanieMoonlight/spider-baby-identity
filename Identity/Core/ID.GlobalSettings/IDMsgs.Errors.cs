using StringHelpers;

namespace ID.GlobalSettings;

#pragma warning disable IDE1006 // Naming Styles
public partial class IDGlobalSettingsMsgs
{

    public static class Error
    {

        public static class Setup
        {
            public const string MISSING_OPTIONS = "You must supply some options for setup.";
            public const string MISSING_APP_NAME = "You must supply a name for the application that is using MyId. It will be used in emails and the like.";
            public const string MISSING_ASSYMETRIC_PRIVATE_KEY = "You must supply a private key when using asymmetrically signed JWTs";
            public const string MISSING_ASSYMETRIC_PUBLIC_KEY = "You must supply a public key when using asymmetrically signed JWTs";
            public const string MISSING_BRAND_COLORS = "You must supply a brand color for your email templates. (Hex format)";
            public const string MISSING_CONFIGURATION = "You must supply a configuration";
            public const string MISSING_CONNECTION_STRING = "You must supply a Connection String";
            public const string MISSING_EMAIL_FROM_ADDRESS = "You must supply a from address for use in account emailing";
            public const string MISSING_CUSTOMER_ACCOUNTS_URL = "You must supply an url for the Customer section of your website. This will be used for registration and forgot password links";
            public const string MISSING_MNTC_ACCOUNTS_URL = "You must supply an url for the Maintenance section of your website. This will be used for registration and forgot password links";


            public static string MISSING_SETUP_DATA(string missingProperty, string library) => $"You must supply a {missingProperty} when using {library}";
        }

    }


}//Cls
#pragma warning restore IDE1006 // Naming Styles
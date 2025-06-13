using Microsoft.Extensions.Configuration;

namespace ID.OAuth.Google.Tests.Setup;

/// <summary>
/// Helper class for loading test configuration from appsettings.Testing.json
/// </summary>
public static class TestConfiguration
{
    private static IConfiguration? _configuration;

    /// <summary>
    /// Gets the test configuration instance, loading it if not already loaded.
    /// </summary>
    public static IConfiguration Configuration
    {
        get
        {
            if (_configuration == null)
            {
                LoadConfiguration();
            }
            return _configuration!;
        }
    }

    /// <summary>
    /// Loads the configuration from appsettings.Testing.json
    /// </summary>
    private static void LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: false);

        _configuration = builder.Build();
    }

    /// <summary>
    /// Gets test JWT tokens from configuration
    /// </summary>
    public static class JwtTokens
    {
        public static string FaultyToken => Configuration["TestData:GoogleJwtTokens:FaultyToken"]!;
        public static string InvalidToken => Configuration["TestData:GoogleJwtTokens:InvalidToken"]!;
        public static string TokenWithWrongAudience => Configuration["TestData:GoogleJwtTokens:TokenWithWrongAudience"]!;
        public static string MalformedToken => Configuration["TestData:GoogleJwtTokens:MalformedToken"]!;
    }

    /// <summary>
    /// Gets test Google OAuth configuration values
    /// </summary>
    public static class GoogleOAuth
    {
        public static string TestClientId => Configuration["TestData:GoogleOAuth:TestClientId"]!;
        public static string CustomClientId => Configuration["TestData:GoogleOAuth:CustomClientId"]!;
    }
}

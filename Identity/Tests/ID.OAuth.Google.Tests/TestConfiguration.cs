using Microsoft.Extensions.Configuration;

namespace ID.OAuth.Google.Tests;

/// <summary>
/// Helper class to load test configuration from appsettings.Testing.json
/// </summary>
public static class TestConfiguration
{
    private static IConfiguration? _configuration;
    
    /// <summary>
    /// Gets the test configuration loaded from appsettings.Testing.json
    /// </summary>
    public static IConfiguration Configuration
    {
        get
        {
            if (_configuration == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: false);
                
                _configuration = builder.Build();
            }
            
            return _configuration;
        }
    }
    
    /// <summary>
    /// Gets test JWT tokens from configuration
    /// </summary>
    public static class JwtTokens
    {
        /// <summary>
        /// A faulty JWT token for testing invalid token scenarios
        /// </summary>
        public static string FaultyToken => Configuration["TestData:GoogleJwtTokens:FaultyToken"] 
            ?? throw new InvalidOperationException("FaultyToken not found in test configuration");
        
        /// <summary>
        /// An obviously invalid token for basic validation tests
        /// </summary>
        public static string InvalidToken => Configuration["TestData:GoogleJwtTokens:InvalidToken"] 
            ?? throw new InvalidOperationException("InvalidToken not found in test configuration");
        
        /// <summary>
        /// A token with wrong audience for testing audience validation
        /// </summary>
        public static string TokenWithWrongAudience => Configuration["TestData:GoogleJwtTokens:TokenWithWrongAudience"] 
            ?? throw new InvalidOperationException("TokenWithWrongAudience not found in test configuration");
        
        /// <summary>
        /// A completely malformed token that doesn't even look like JWT
        /// </summary>
        public static string MalformedToken => Configuration["TestData:GoogleJwtTokens:MalformedToken"] 
            ?? throw new InvalidOperationException("MalformedToken not found in test configuration");
    }
    
    /// <summary>
    /// Gets Google OAuth test configuration
    /// </summary>
    public static class GoogleOAuth
    {
        /// <summary>
        /// Test client ID for basic testing
        /// </summary>
        public static string TestClientId => Configuration["TestData:GoogleOAuth:TestClientId"] 
            ?? throw new InvalidOperationException("TestClientId not found in test configuration");
        
        /// <summary>
        /// Custom client ID for specific test scenarios
        /// </summary>
        public static string CustomClientId => Configuration["TestData:GoogleOAuth:CustomClientId"] 
            ?? throw new InvalidOperationException("CustomClientId not found in test configuration");
    }
}

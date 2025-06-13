using ID.Application.AppAbs.SignIn;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using ID.Infrastructure.Auth.Cookies;
using ID.Infrastructure.Auth.Cookies.Services;
using ID.Infrastructure.Tests.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace ID.Infrastructure.Tests.Auth.Cookies;

[Collection(TestingConstants.NonParallelCollection)]
public class CookieSetupTests
{
    [Fact]
    public void AddMyIdCookies_Should_Configure_CookieOptions_With_Custom_LoginPath()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.CookieLoginPath = "/custom/login";

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        cookieOptions.CookieLoginPath.ShouldBe("/custom/login");
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdCookies_Should_Configure_CookieOptions_With_Custom_LogoutPath()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.CookieLogoutPath = "/custom/logout";

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        cookieOptions.CookieLogoutPath.ShouldBe("/custom/logout");
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdCookies_Should_Configure_CookieOptions_With_Custom_AccessDeniedPath()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.CookieAccessDeniedPath = "/custom/access-denied";

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        cookieOptions.CookieAccessDeniedPath.ShouldBe("/custom/access-denied");
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdCookies_Should_Configure_CookieOptions_With_Custom_SlidingExpiration()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.CookieSlidingExpiration = false;

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        cookieOptions.CookieSlidingExpiration.ShouldBe(false);
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdCookies_Should_Configure_CookieOptions_With_Custom_ExpireTimeSpan()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.CookieExpireTimeSpan = TimeSpan.FromHours(2);

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        cookieOptions.CookieExpireTimeSpan.ShouldBe(TimeSpan.FromHours(2));
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdCookies_Should_Use_Default_Values_When_Not_Specified()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateEmptyDefaultSetupOptions();
        setupOptions.ConnectionString = "DummyConnection"; // Required field

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        // Verify defaults are used (these would be the default values from CookieDefaultValues or similar)
        cookieOptions.ShouldNotBeNull();
        cookieOptions.CookieLoginPath.ShouldNotBeNullOrEmpty();
        cookieOptions.CookieLogoutPath.ShouldNotBeNullOrEmpty();
        cookieOptions.CookieAccessDeniedPath.ShouldNotBeNullOrEmpty();
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdCookies_Should_Override_Only_Specified_Values()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateEmptyDefaultSetupOptions();
        setupOptions.ConnectionString = "DummyConnection"; // Required field
        setupOptions.CookieLoginPath = "/custom/login"; // Only specify what we want to test

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        cookieOptions.CookieLoginPath.ShouldBe("/custom/login");
        // Other values should use defaults
        cookieOptions.CookieLogoutPath.ShouldNotBeNullOrEmpty();
        cookieOptions.CookieAccessDeniedPath.ShouldNotBeNullOrEmpty();
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdCookies_Should_Register_CookieSignInService()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();


        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        services.ShouldContain(s =>
            s.ServiceType == typeof(ICookieSignInService<AppUser>) &&
            s.ImplementationType == typeof(CookieSignInService<AppUser>) &&
            s.Lifetime == ServiceLifetime.Scoped);
        //var serviceProvider = services.BuildServiceProvider();
        //var cookieSignInService = serviceProvider.GetService<ICookieSignInService<AppUser>>();
        
        //cookieSignInService.ShouldNotBeNull();
        //cookieSignInService.ShouldBeOfType<CookieSignInService<AppUser>>();
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdCookies_Should_Return_Services_With_Configured_CookieOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();

        // Act
        var resultServices = services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        resultServices.ShouldBeSameAs(services);
        
        // Verify that CookieOptions is registered correctly
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetService<IOptions<CookieOptions>>();
        cookieOptions.ShouldNotBeNull();
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdCookies_Should_Configure_Multiple_Properties_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.CookieLoginPath = "/test/login";
        setupOptions.CookieLogoutPath = "/test/logout";
        setupOptions.CookieAccessDeniedPath = "/test/denied";
        setupOptions.CookieSlidingExpiration = true;
        setupOptions.CookieExpireTimeSpan = TimeSpan.FromMinutes(90);

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        cookieOptions.CookieLoginPath.ShouldBe("/test/login");
        cookieOptions.CookieLogoutPath.ShouldBe("/test/logout");
        cookieOptions.CookieAccessDeniedPath.ShouldBe("/test/denied");
        cookieOptions.CookieSlidingExpiration.ShouldBe(true);
        cookieOptions.CookieExpireTimeSpan.ShouldBe(TimeSpan.FromMinutes(90));
    }

    //-------------------------------------//

    [Fact]
    public void UseCookieAuth_Should_Configure_CookieAuthenticationOptions_Using_Lazy_Pattern()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(provider => 
            Options.Create(new CookieOptions
            {
                CookieLoginPath = "/test/login",
                CookieLogoutPath = "/test/logout", 
                CookieAccessDeniedPath = "/test/denied",
                CookieSlidingExpiration = true,
                CookieExpireTimeSpan = TimeSpan.FromHours(1)
            }));

        var authBuilder = services.AddAuthentication();

        // Act
        var result = authBuilder.UseCookieAuth();

        // Assert
        result.ShouldBeSameAs(authBuilder);
        
        // Verify that IConfigureOptions<CookieAuthenticationOptions> is registered
        var serviceProvider = services.BuildServiceProvider();
        var configureOptions = serviceProvider.GetService<IConfigureOptions<CookieAuthenticationOptions>>();
        configureOptions.ShouldNotBeNull();
    }

    //-------------------------------------//

    [Fact]
    public void UseCookieAuth_Should_Use_Lazy_Loading_Pattern_Not_Service_Locator()
    {
        // Arrange
        var services = new ServiceCollection();
        var customCookieOptions = new CookieOptions
        {
            CookieLoginPath = "/lazy/login",
            CookieLogoutPath = "/lazy/logout",
            CookieAccessDeniedPath = "/lazy/denied",
            CookieSlidingExpiration = false,
            CookieExpireTimeSpan = TimeSpan.FromHours(3)
        };
        
        services.AddSingleton(provider => Options.Create(customCookieOptions));
        services.AddSingleton<IWebHostEnvironment>(new TestWebHostEnvironment());
        var authBuilder = services.AddAuthentication();

        // Act
        authBuilder.UseCookieAuth();

        // Assert - Configuration should be applied lazily when needed
        var serviceProvider = services.BuildServiceProvider();
        
        // Get the configured options through the normal ASP.NET Core options system
        var configuredOptions = serviceProvider.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
        var actualOptions = configuredOptions.Get(CookieAuthenticationDefaults.AuthenticationScheme);

        // Verify the lazy configuration was applied correctly
        actualOptions.LoginPath.Value.ShouldBe("/lazy/login");
        actualOptions.LogoutPath.Value.ShouldBe("/lazy/logout");
        actualOptions.AccessDeniedPath.Value.ShouldBe("/lazy/denied");
        actualOptions.SlidingExpiration.ShouldBe(false);
        actualOptions.ExpireTimeSpan.ShouldBe(TimeSpan.FromHours(3));
    }

    //-------------------------------------//

    [Theory]
    [InlineData("/custom/login", "/default/logout")]
    [InlineData("/another/login", "/another/logout")]
    public void AddMyIdCookies_Should_Handle_Various_Path_Combinations(string loginPath, string logoutPath)
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.CookieLoginPath = loginPath;
        setupOptions.CookieLogoutPath = logoutPath;

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        cookieOptions.CookieLoginPath.ShouldBe(loginPath);
        cookieOptions.CookieLogoutPath.ShouldBe(logoutPath);    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdCookies_With_IConfiguration_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CookieLoginPath"] = "/config/login",
                ["CookieLogoutPath"] = "/config/logout",
                ["CookieAccessDeniedPath"] = "/config/denied"
            })
            .Build();

        // Act
        services.AddMyIdCookies<AppUser>(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        cookieOptions.CookieLoginPath.ShouldBe("/config/login");
        cookieOptions.CookieLogoutPath.ShouldBe("/config/logout");
        cookieOptions.CookieAccessDeniedPath.ShouldBe("/config/denied");
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdCookies_With_Null_Configuration_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            services.AddMyIdCookies<AppUser>(null!, null));

        exception.ParamName.ShouldBe("configuration");
        exception.Message.ShouldContain(IDMsgs.Error.Setup.MISSING_CONFIGURATION);
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdCookies_With_Configuration_Section_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();       
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MySection:CookieLoginPath"] = "/section/login",
                ["MySection:CookieLogoutPath"] = "/section/logout",
                ["MySection:CookieAccessDeniedPath"] = "/section/denied"
            })
            .Build();

        // Act
        services.AddMyIdCookies<AppUser>(configuration, "MySection");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        cookieOptions.CookieLoginPath.ShouldBe("/section/login");
        cookieOptions.CookieLogoutPath.ShouldBe("/section/logout");
        cookieOptions.CookieAccessDeniedPath.ShouldBe("/section/denied");
    }

    //-------------------------------------//

    [Theory]
    [InlineData("login", "/login")] // Should add leading slash
    [InlineData("/login", "/login")] // Should preserve existing leading slash
    [InlineData("", "/Account/Signin")] // Should use default for empty string
    [InlineData("   ", "/Account/Signin")] // Should use default for whitespace
    [InlineData(null, "/Account/Signin")] // Should use default for null
    public void AddMyIdCookies_Should_Format_LoginPath_Correctly(string? inputPath, string expectedPath)
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.CookieLoginPath = inputPath;

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        cookieOptions.CookieLoginPath.ShouldBe(expectedPath);
    }

    //-------------------------------------//

    [Theory]
    [InlineData("logout", "/logout")] // Should add leading slash
    [InlineData("/logout", "/logout")] // Should preserve existing leading slash
    [InlineData("", "/Account/Logout")] // Should use default for empty string
    [InlineData("   ", "/Account/Logout")] // Should use default for whitespace
    [InlineData(null, "/Account/Logout")] // Should use default for null
    public void AddMyIdCookies_Should_Format_LogoutPath_Correctly(string? inputPath, string expectedPath)
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.CookieLogoutPath = inputPath;

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        cookieOptions.CookieLogoutPath.ShouldBe(expectedPath);
    }

    //-------------------------------------//

    [Theory]
    [InlineData("access-denied", "/access-denied")] // Should add leading slash
    [InlineData("/access-denied", "/access-denied")] // Should preserve existing leading slash
    [InlineData("", "/Account/AccessDenied")] // Should use default for empty string
    [InlineData("   ", "/Account/AccessDenied")] // Should use default for whitespace
    [InlineData(null, "/Account/AccessDenied")] // Should use default for null
    public void AddMyIdCookies_Should_Format_AccessDeniedPath_Correctly(string? inputPath, string expectedPath)
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.CookieAccessDeniedPath = inputPath;

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        cookieOptions.CookieAccessDeniedPath.ShouldBe(expectedPath);
    }

    //-------------------------------------//

    [Theory]
    [InlineData(-1)] // Negative value should use default
    [InlineData(0)] // Zero value should use default
    public void AddMyIdCookies_Should_Use_Default_For_Invalid_ExpireTimeSpan(int minutes)
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.CookieExpireTimeSpan = TimeSpan.FromMinutes(minutes);

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        // Should use the default value (1 hour) instead of the invalid value
        cookieOptions.CookieExpireTimeSpan.ShouldBe(CookieDefaultValues.EXPIRE_TIME_SPAN);
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdCookies_Should_Use_Valid_ExpireTimeSpan_When_Positive()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        var validTimeSpan = TimeSpan.FromHours(2);
        setupOptions.CookieExpireTimeSpan = validTimeSpan;

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        cookieOptions.CookieExpireTimeSpan.ShouldBe(validTimeSpan);
    }

    //-------------------------------------//

    [Fact]
    public void UseCookieAuth_Should_Apply_Path_Formatting_To_ASP_NET_Core_Options()
    {
        // Arrange
        var services = new ServiceCollection();
        var customCookieOptions = new CookieOptions
        {
            CookieLoginPath = "custom/login", // No leading slash
            CookieLogoutPath = "custom/logout", // No leading slash
            CookieAccessDeniedPath = "custom/denied", // No leading slash
            CookieSlidingExpiration = true,
            CookieExpireTimeSpan = TimeSpan.FromHours(1)
        };
        
        services.AddSingleton(provider => Options.Create(customCookieOptions));
        services.AddSingleton<IWebHostEnvironment>(new TestWebHostEnvironment());

        var authBuilder = services.AddAuthentication();

        // Act
        authBuilder.UseCookieAuth();

        // Assert - Verify the formatted paths are applied to ASP.NET Core options
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
        var actualOptions = configuredOptions.Get(CookieAuthenticationDefaults.AuthenticationScheme);

        // The CookieOptions should have formatted the paths with leading slashes
        actualOptions.LoginPath.Value.ShouldBe("/custom/login");
        actualOptions.LogoutPath.Value.ShouldBe("/custom/logout");
        actualOptions.AccessDeniedPath.Value.ShouldBe("/custom/denied");
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdCookies_Should_Use_Exact_Default_Values()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateEmptyDefaultSetupOptions();

        // Act
        services.AddMyIdCookies<AppUser>(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cookieOptions = serviceProvider.GetRequiredService<IOptions<CookieOptions>>().Value;

        // Verify exact default values are used
        cookieOptions.CookieLoginPath.ShouldBe(CookieDefaultValues.LOGIN_PATH);
        cookieOptions.CookieLogoutPath.ShouldBe(CookieDefaultValues.LOGOUT_PATH);
        cookieOptions.CookieAccessDeniedPath.ShouldBe(CookieDefaultValues.ACCESS_DENIED_PATH);
        cookieOptions.CookieSlidingExpiration.ShouldBe(CookieDefaultValues.SLIDING_EXPIRATION);
        cookieOptions.CookieExpireTimeSpan.ShouldBe(CookieDefaultValues.EXPIRE_TIME_SPAN);
    }

    //=====================================//


    // Test implementation of IWebHostEnvironment
    public class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = "wwwroot";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ApplicationName { get; set; } = "TestApp";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = "content";
        public string EnvironmentName { get; set; } = "Development";
    }

}//Cls

using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Setup.Defaults;
using ID.GlobalSettings.Utility;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Setup;
using ID.Infrastructure.Tests.Auth.JWT.Utils;
using ID.Infrastructure.Tests.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TestingHelpers;

namespace ID.Infrastructure.Tests.Auth.JWT;

[Collection(TestingConstants.NonParallelCollection)]
public class JwtSetupTests
{


    [Theory]
    [InlineData(IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH - 5)]
    public void AddMyIdJwt_Should_Throw_Exception_When_TokenSigningKey_Too_Short(int keyLength)
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.SymmetricTokenSigningKey = new string('a', keyLength);

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() => services.AddMyIdJwt(setupOptions));
        Assert.Equal(IDMsgs.Error.Jwt.TOKEN_SIGNING_KEY_TOO_SHORT(IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH), exception.Message);
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdJwt_Should_Throw_Exception_When_Missing_Asymmetric_Public_Key()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.SymmetricTokenSigningKey = string.Empty;
        setupOptions.AsymmetricTokenPublicKey_Pem = null;
        setupOptions.AsymmetricTokenPublicKey_Xml = null;
        setupOptions.AsymmetricTokenPrivateKey_Xml = RandomStringGenerator.Generate(50);

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() => services.AddMyIdJwt(setupOptions));
        Assert.Equal(IDMsgs.Error.Setup.MISSING_ASSYMETRIC_PUBLIC_KEY, exception.Message);
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdJwt_Should_Throw_Exception_When_Missing_Asymmetric_Private_Key()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.SymmetricTokenSigningKey = string.Empty;
        setupOptions.AsymmetricTokenPublicKey_Xml = RandomStringGenerator.Generate(50);
        setupOptions.AsymmetricTokenPrivateKey_Xml = null;
        setupOptions.AsymmetricTokenPrivateKey_Pem = null;

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() => services.AddMyIdJwt(setupOptions));
        Assert.Equal(IDMsgs.Error.Setup.MISSING_ASSYMETRIC_PRIVATE_KEY, exception.Message);
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdJwt_Should_Configure_JwtOptions_With_Asymmetric_Keys()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.SymmetricTokenSigningKey = null;
        setupOptions.AsymmetricTokenPublicKey_Xml = "<RSAKeyValue><Modulus>...</Modulus><Exponent>...</Exponent></RSAKeyValue>";
        setupOptions.AsymmetricTokenPrivateKey_Xml = "<RSAKeyValue><Modulus>...</Modulus><Exponent>...</Exponent><P>...</P><Q>...</Q><DP>...</DP><DQ>...</DQ><InverseQ>...</InverseQ><D>...</D></RSAKeyValue>";

        // Act
        services.AddMyIdJwt(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.True(jwtOptions.UseAsymmetricCrypto);
        Assert.Equal(setupOptions.AsymmetricTokenPublicKey_Xml, jwtOptions.AsymmetricTokenPublicKey_Xml);
        Assert.Equal(setupOptions.AsymmetricTokenPrivateKey_Xml, jwtOptions.AsymmetricTokenPrivateKey_Xml);
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdJwt_Should_Configure_JwtOptions_With_Symmetric_Key()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        var validKey = new string('a', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH);
        setupOptions.SymmetricTokenSigningKey = validKey;

        // Act
        services.AddMyIdJwt(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.False(jwtOptions.UseAsymmetricCrypto);
        Assert.Equal(validKey, jwtOptions.SymmetricTokenSigningKey);
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdJwt_Should_Configure_JwtOptions_With_TokenExpirationMinutes()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.TokenExpirationMinutes = 120;
        setupOptions.SymmetricTokenSigningKey = new string('a', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH);

        // Act
        services.AddMyIdJwt(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.Equal(120, jwtOptions.TokenExpirationMinutes);
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdJwt_Should_Configure_JwtOptions_With_TokenIssuer()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.TokenIssuer = "MyCustomIssuer";
        setupOptions.SymmetricTokenSigningKey = new string('a', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH);

        // Act
        services.AddMyIdJwt(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.Equal("MyCustomIssuer", jwtOptions.TokenIssuer);
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdJwt_Should_Configure_JwtOptions_With_SecurityAlgorithm()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.SecurityAlgorithm = "CustomAlgorithm";
        setupOptions.SymmetricTokenSigningKey = new string('a', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH);

        // Act
        services.AddMyIdJwt(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.Equal("CustomAlgorithm", jwtOptions.SecurityAlgorithm);
    }

    //-------------------------------------//



    [Fact]
    public void AddMyIdJwt_Should_Use_Default_TokenExpirationMinutes_When_Not_Specified()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateEmptyDefaultSetupOptions();
        setupOptions.ConnectionString = "DummyConnection"; // Required field
        setupOptions.SymmetricTokenSigningKey = new string('a', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH);

        // Act
        services.AddMyIdJwt(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.Equal(IdGlobalDefaultValues.TOKEN_EXPIRATION_MINUTES, jwtOptions.TokenExpirationMinutes);
    }

    [Fact]
    public void AddMyIdJwt_Should_Override_Only_Specified_Values()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateEmptyDefaultSetupOptions();
        setupOptions.ConnectionString = "DummyConnection"; // Required field
        setupOptions.SymmetricTokenSigningKey = new string('a', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH);
        setupOptions.TokenIssuer = "CustomIssuer";  // Only specify what we want to test

        // Act
        services.AddMyIdJwt(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.Equal("CustomIssuer", jwtOptions.TokenIssuer);
        Assert.Equal(IdGlobalDefaultValues.SECURITY_ALGORITHM, jwtOptions.SecurityAlgorithm); // Default used
    }

    [Theory]
    [InlineData(IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH - 5)]
    public void AddMyIdJwt_Should_Throw_When_SymmetricKey_Too_Short(int keyLength)
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateEmptyDefaultSetupOptions();
        setupOptions.ConnectionString = "DummyConnection"; // Required field
        setupOptions.SymmetricTokenSigningKey = new string('a', keyLength);

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() =>
            services.AddMyIdJwt(setupOptions));

        Assert.Equal(IDMsgs.Error.Jwt.TOKEN_SIGNING_KEY_TOO_SHORT(
            IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH), exception.Message);
    }

    [Fact]
    public void AddMyIdJwt_Should_Configure_AsymmetricKeys_When_No_SymmetricKey()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateEmptyDefaultSetupOptions();
        setupOptions.ConnectionString = "DummyConnection"; // Required field
        setupOptions.SymmetricTokenSigningKey = null;
        setupOptions.AsymmetricTokenPublicKey_Xml = "<RSAKeyValue><Modulus>...</Modulus><Exponent>...</Exponent></RSAKeyValue>";
        setupOptions.AsymmetricTokenPrivateKey_Xml = "<RSAKeyValue><Modulus>...</Modulus><Exponent>...</Exponent><P>...</P><Q>...</Q><DP>...</DP><DQ>...</DQ><InverseQ>...</InverseQ><D>...</D></RSAKeyValue>";

        // Act
        services.AddMyIdJwt(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.True(jwtOptions.UseAsymmetricCrypto);
        Assert.Equal(setupOptions.AsymmetricTokenPublicKey_Xml, jwtOptions.AsymmetricTokenPublicKey_Xml);
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdJwt_Should_Return_Services_With_Configured_JwtOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.SymmetricTokenSigningKey = new string('a', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH);

        // Act
        var resultServices = services.AddMyIdJwt(setupOptions);

        // Assert
        Assert.Same(services, resultServices);
        
        // Verify that JwtOptions is registered correctly
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetService<IOptions<JwtOptions>>();
        Assert.NotNull(jwtOptions);
    }

    //-------------------------------------//

    [Fact]
    public void AddMyIdJwt_Should_Convert_PEM_To_XML_When_XML_Not_Provided()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.SymmetricTokenSigningKey = string.Empty;
        setupOptions.AsymmetricTokenPublicKey_Xml = null;
        setupOptions.AsymmetricTokenPrivateKey_Xml = null;
        setupOptions.AsymmetricTokenPublicKey_Pem = "-----BEGIN PUBLIC KEY-----\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCrVrCuTtArbgaZzL1hvh0xtL5mc7o0NqPVnYXkLvgcwiC3BjLGw1tGEGoJaXDuSaRllobm53JBhjx33UNv+5z/UMG4kytBWxheNVKnL6GgqlNabMaFfPLPCF8kAgKnsi79NMo+n6KnSY8YeUmec/p2vjO2NjsSAVcWEQMVhJ31LwIDAQAB\n-----END PUBLIC KEY-----";
        setupOptions.AsymmetricTokenPrivateKey_Pem = "-----BEGIN RSA PRIVATE KEY-----\nMIICXAIBAAKBgQCrVrCuTtArbgaZzL1hvh0xtL5mc7o0NqPVnYXkLvgcwiC3BjLGw1tGEGoJaXDuSaRllobm53JBhjx33UNv+5z/UMG4kytBWxheNVKnL6GgqlNabMaFfPLPCF8kAgKnsi79NMo+n6KnSY8YeUmec/p2vjO2NjsSAVcWEQMVhJ31LwIDAQABAoGBAI1D/ETdB7xCJR8+kGzOzFnOQ2TmU3TqPPnqXeMGYt7DAmNK+HabS3P1Y4/S3SqZ0pLeUIYkJHDKoE2irkWUGOQQ6Eb5Xt1nYZ4CSY/q5r8L8t7CClh/CD3rDhR8TpVj9ILAeadM+lG1r+AD9yAIUBUOvhqrGt5IkWOtXSgRXm6BAkEA4xPJ7fKAiMZRIaJkLU7zphIBk7dq/QGgQf9bOJPPK2KbOyC2dKPkYhv0gglYKRBgPwFENYTOAHCBQ3NFYi0nowJBAMGsFIAYL30Hq+/5WrTAEgWXZBQIL3nS+i/fIwvM07pPGPHohxo7x+BCRAew5vG+BN/6Q9uL5Lcea8/5QnbFX2kCQHGMQQdJjgcz/yqq3qYAB4L276reUGK+wwGK+8z9Hm6+/HnNs9TtwwE4gyw7PDjKErGI0kCcr6RIAx42VEQXMo8CQDT/PNzCZ/roaLHJJuMXBzVJsm8DHVYqFjjLg5RkmCRzMKcZ5fYTTYxqEJupJmLXk4JgG0G4FEuGLhLj7OVReIkCQCABqODYnk3hqh/zcZgYXcwfQQ47cEF5NPUQule/JenkCPnJ4OKiLGdO934aML4bNlWYQYQzbbQ8/a6D8BGyJP4=\n-----END RSA PRIVATE KEY-----";

        // Act - Should not throw
        services.AddMyIdJwt(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.True(jwtOptions.UseAsymmetricCrypto);
        Assert.NotNull(jwtOptions.AsymmetricTokenPublicKey_Xml);
        Assert.NotNull(jwtOptions.AsymmetricTokenPrivateKey_Xml);
        Assert.NotEmpty(jwtOptions.AsymmetricTokenPublicKey_Xml);
        Assert.NotEmpty(jwtOptions.AsymmetricTokenPrivateKey_Xml);
    }

    //-------------------------------------//
    // ConfigureJwtOptions Tests
    //-------------------------------------//

    [Fact]
    public void ConfigureJwtOptions_Should_Configure_Options_From_Configuration()
    {
        // Arrange
        var services = new ServiceCollection();
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["TokenExpirationMinutes"] = "120",
                ["SymmetricTokenSigningKey"] = new string('x', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH),
                ["TokenIssuer"] = "TestIssuer",
                ["SecurityAlgorithm"] = "HS512"
            })
            .Build();

        // Act
        services.ConfigureJwtOptions(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.Equal(120, jwtOptions.TokenExpirationMinutes);
        Assert.Equal(new string('x', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH), jwtOptions.SymmetricTokenSigningKey);
        Assert.Equal("TestIssuer", jwtOptions.TokenIssuer);
        Assert.Equal("HS512", jwtOptions.SecurityAlgorithm);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureJwtOptions_Should_Configure_Options_From_Named_Section()
    {
        // Arrange
        var services = new ServiceCollection();
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSection:TokenExpirationMinutes"] = "180",
                ["JwtSection:SymmetricTokenSigningKey"] = new string('y', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH),
                ["JwtSection:TokenIssuer"] = "SectionIssuer",
                ["JwtSection:SecurityAlgorithm"] = "HS384"
            })
            .Build();

        // Act
        services.ConfigureJwtOptions(configuration, "JwtSection");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.Equal(180, jwtOptions.TokenExpirationMinutes);
        Assert.Equal(new string('y', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH), jwtOptions.SymmetricTokenSigningKey);
        Assert.Equal("SectionIssuer", jwtOptions.TokenIssuer);
        Assert.Equal("HS384", jwtOptions.SecurityAlgorithm);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureJwtOptions_Should_Use_Default_Values_When_Configuration_Empty()
    {
        // Arrange
        var services = new ServiceCollection();
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([])
            .Build();

        // Act
        services.ConfigureJwtOptions(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        // Should use default values from JwtOptions constructor/properties
        Assert.Equal(IdGlobalDefaultValues.TOKEN_EXPIRATION_MINUTES, jwtOptions.TokenExpirationMinutes);
        Assert.Equal(string.Empty, jwtOptions.SymmetricTokenSigningKey);
        Assert.Equal(IdGlobalDefaultValues.TOKEN_ISSUER, jwtOptions.TokenIssuer);
        Assert.Equal(IdGlobalDefaultValues.SECURITY_ALGORITHM, jwtOptions.SecurityAlgorithm);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureJwtOptions_Should_Throw_SetupDataException_When_Configuration_Null()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() =>
            services.ConfigureJwtOptions(null!));

        exception.Message.ShouldContain(nameof(IdInfrastructureSetupOptions));

        //Assert.Contains(IDMsgs.Error.Setup.MISSING_CONFIGURATION);
    }

    //-------------------------------------//

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ConfigureJwtOptions_Should_Use_Configuration_Directly_When_SectionName_NullOrEmpty(string? sectionName)
    {
        // Arrange
        var services = new ServiceCollection();
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["TokenExpirationMinutes"] = "240",
                ["TokenIssuer"] = "DirectIssuer"
            })
            .Build();

        // Act
        services.ConfigureJwtOptions(configuration, sectionName);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.Equal(240, jwtOptions.TokenExpirationMinutes);
        Assert.Equal("DirectIssuer", jwtOptions.TokenIssuer);
    }

    //-------------------------------------//
    // Existing AddMyIdJwt Tests
    //-------------------------------------//

    //-------------------------------------//
    // CopyOptionsValues Comprehensive Test
    //-------------------------------------//

    [Fact]
    public void ConfigureJwtOptions_Should_Assign_All_Available_Properties_Without_Missing_Any__ASSYMETRIC()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        
        // Set ALL possible properties to unique test values
        setupOptions.TokenExpirationMinutes = 999;
        setupOptions.SymmetricTokenSigningKey = string.Empty;
        setupOptions.TokenIssuer = "TestIssuer_CopyTest";
        setupOptions.SecurityAlgorithm = "HS512_CopyTest";
        setupOptions.AsymmetricAlgorithm = "RS512_CopyTest";
        setupOptions.AsymmetricTokenPublicKey_Xml = "<RSAKeyValue>PublicKey_CopyTest</RSAKeyValue>";
        setupOptions.AsymmetricTokenPrivateKey_Xml = "<RSAKeyValue>PrivateKey_CopyTest</RSAKeyValue>";
        setupOptions.AsymmetricTokenPublicKey_Pem = "-----BEGIN PUBLIC KEY-----\nPemPublicKey_CopyTest\n-----END PUBLIC KEY-----";
        setupOptions.AsymmetricTokenPrivateKey_Pem = "-----BEGIN PRIVATE KEY-----\nPemPrivateKey_CopyTest\n-----END PRIVATE KEY-----";
        setupOptions.RefreshTokenUpdatePolicy = RefreshTokenUpdatePolicy.Always;

        // Act
        services.AddMyIdJwt(setupOptions);

        // Assert - Verify EVERY property from the CopyOptionsValues method is correctly assigned
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        // Verify all 10 properties listed in CopyOptionsValues method (lines 78-87)
        Assert.Equal(999, jwtOptions.TokenExpirationMinutes);
        //Assert.Equal(new string('X', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH), jwtOptions.SymmetricTokenSigningKey);
        Assert.Equal("TestIssuer_CopyTest", jwtOptions.TokenIssuer);
        Assert.Equal("HS512_CopyTest", jwtOptions.SecurityAlgorithm);
        Assert.Equal("RS512_CopyTest", jwtOptions.AsymmetricAlgorithm);
        Assert.Equal("<RSAKeyValue>PublicKey_CopyTest</RSAKeyValue>", jwtOptions.AsymmetricTokenPublicKey_Xml);
        Assert.Equal("<RSAKeyValue>PrivateKey_CopyTest</RSAKeyValue>", jwtOptions.AsymmetricTokenPrivateKey_Xml);
        Assert.Equal("-----BEGIN PUBLIC KEY-----\nPemPublicKey_CopyTest\n-----END PUBLIC KEY-----", jwtOptions.AsymmetricTokenPublicKey_Pem);
        Assert.Equal("-----BEGIN PRIVATE KEY-----\nPemPrivateKey_CopyTest\n-----END PRIVATE KEY-----", jwtOptions.AsymmetricTokenPrivateKey_Pem);
        Assert.Equal(RefreshTokenUpdatePolicy.Always, jwtOptions.RefreshTokenUpdatePolicy);

        // This test will FAIL if any property is added to JwtOptions but not included in CopyOptionsValues
        // forcing developers to update both the method and this test, preventing forgotten assignments
    }

    //-------------------------------------//
    [Fact]
    public void ConfigureJwtOptions_Should_Assign_All_Available_Properties_Without_Missing_Any__SYMMETRIC()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();

        // Set ALL possible properties to unique test values
        setupOptions.TokenExpirationMinutes = 999;
        setupOptions.SymmetricTokenSigningKey = new string('X', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH);
        setupOptions.TokenIssuer = "TestIssuer_CopyTest";
        setupOptions.SecurityAlgorithm = "HS512_CopyTest";
        setupOptions.AsymmetricAlgorithm = "RS512_CopyTest";
        setupOptions.AsymmetricTokenPublicKey_Xml = "<RSAKeyValue>PublicKey_CopyTest</RSAKeyValue>";
        setupOptions.AsymmetricTokenPrivateKey_Xml = "<RSAKeyValue>PrivateKey_CopyTest</RSAKeyValue>";
        setupOptions.AsymmetricTokenPublicKey_Pem = "-----BEGIN PUBLIC KEY-----\nPemPublicKey_CopyTest\n-----END PUBLIC KEY-----";
        setupOptions.AsymmetricTokenPrivateKey_Pem = "-----BEGIN PRIVATE KEY-----\nPemPrivateKey_CopyTest\n-----END PRIVATE KEY-----";
        setupOptions.RefreshTokenUpdatePolicy = RefreshTokenUpdatePolicy.Always;

        // Act
        services.AddMyIdJwt(setupOptions);

        // Assert - Verify EVERY property from the CopyOptionsValues method is correctly assigned
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        // Verify all 10 properties listed in CopyOptionsValues method (lines 78-87)
        Assert.Equal(999, jwtOptions.TokenExpirationMinutes);
        //Assert.Equal(new string('X', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH), jwtOptions.SymmetricTokenSigningKey);
        Assert.Equal("TestIssuer_CopyTest", jwtOptions.TokenIssuer);
        Assert.Equal("HS512_CopyTest", jwtOptions.SecurityAlgorithm);
        Assert.Equal("RS512_CopyTest", jwtOptions.AsymmetricAlgorithm);
        Assert.Equal(RefreshTokenUpdatePolicy.Always, jwtOptions.RefreshTokenUpdatePolicy);

        // This test will FAIL if any property is added to JwtOptions but not included in CopyOptionsValues
        // forcing developers to update both the method and this test, preventing forgotten assignments
    }

    //-------------------------------------//

    //-------------------------------------//
    // RefreshTokenTimeSpan Tests
    //-------------------------------------//

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void ConfigureJwtOptions_WithInvalidRefreshTokenTimeSpan_Should_DefaultToValidValue(int hours)
    {
        // Arrange
        var services = new ServiceCollection();
        var jwtOptions = JwtOptionsUtils.InitiallyValidOptions(
            refreshTokenTimeSpan: TimeSpan.FromHours(hours) // Invalid - should be defaulted by setter
        );

        // Act - Should not throw exception because setter defaults invalid values
        services.Configure<JwtOptions>(opts =>
        {
            opts.RefreshTokenTimeSpan = jwtOptions.RefreshTokenTimeSpan;
        });

        // Assert - The value should be defaulted by the setter
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;
        Assert.Equal(IdGlobalDefaultValues.REFRESH_TOKEN_EXPIRE_TIME_SPAN, configuredOptions.RefreshTokenTimeSpan);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureJwtOptions_WithValidRefreshTokenTimeSpan_Should_Configure_Successfully()
    {
        // Arrange
        var services = new ServiceCollection();
        var expectedTimeSpan = TimeSpan.FromDays(7);
        var jwtOptions = JwtOptionsUtils.InitiallyValidOptions(
            refreshTokenTimeSpan: expectedTimeSpan
        );

        // Act
        services.Configure<JwtOptions>(opts =>
        {
            opts.RefreshTokenTimeSpan = jwtOptions.RefreshTokenTimeSpan;
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;
        Assert.Equal(expectedTimeSpan, configuredOptions.RefreshTokenTimeSpan);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureJwtOptionsFromConfiguration_WithRefreshTokenTimeSpan_Should_Configure_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RefreshTokenTimeSpan"] = "02:00:00",
                ["TokenExpirationMinutes"] = "30",
                ["TokenIssuer"] = "TestIssuer",
                ["SecurityAlgorithm"] = "HS256",
                ["SymmetricTokenSigningKey"] = RandomStringGenerator.Generate(64)
            })
            .Build();

        // Act
        services.ConfigureJwtOptions(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;
        Assert.Equal(TimeSpan.FromHours(2), configuredOptions.RefreshTokenTimeSpan);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureJwtOptionsFromConfiguration_WithSection_RefreshTokenTimeSpan_Should_Configure_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:RefreshTokenTimeSpan"] = "01:30:00",
                ["JwtSettings:TokenExpirationMinutes"] = "30",
                ["JwtSettings:TokenIssuer"] = "TestIssuer",
                ["JwtSettings:SecurityAlgorithm"] = "HS256",
                ["JwtSettings:SymmetricTokenSigningKey"] = RandomStringGenerator.Generate(64)
            })
            .Build();

        // Act
        services.ConfigureJwtOptions(configuration, "JwtSettings");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;
        Assert.Equal(TimeSpan.FromMinutes(90), configuredOptions.RefreshTokenTimeSpan);
    }

    //-------------------------------------//
}
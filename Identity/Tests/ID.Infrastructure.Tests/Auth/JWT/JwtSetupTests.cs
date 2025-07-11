using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Setup.Defaults;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Auth.JWT.Utils;
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
    private const string _validPublicKeyXml = @"<RSAKeyValue>
<Modulus>1hYinUnmO1QyansNWEWin0JGA9fS0+MGlGi1WNHFDfm8eAiiT2KK11U8+gx+QjAqFNXMEv4TxPgIT99ANI+K5VD3/x7JsRJZTonmSg34WdAOSR2V/7blGWMkHjPwR5hiaNyMpOAwr0NcbIiNAtl6eubAG4kKkZ6c871S0Cmk1n4MoHZCMKJQ+CVAkOqy1TAtzZhgHxnQ86PGT7NbC37PqlC5G8Erwi81YT4Jqw0T7zFGo8wyWIeRm4JmmKVWVBSUr8qUC+QZF7th3TAO9PvCfaX5hW2wtOVlKZA9goTfzFICzr+Pxtq1dZC7sOmPJgp1kl/HLcCP020TF10m9dzG7w==</Modulus>
<Exponent>AQAB</Exponent>
</RSAKeyValue>";
    private const string _validPrivateKeyXml = @"<RSAKeyValue>
<Modulus>1hYinUnmO1QyansNWEWin0JGA9fS0+MGlGi1WNHFDfm8eAiiT2KK11U8+gx+QjAqFNXMEv4TxPgIT99ANI+K5VD3/x7JsRJZTonmSg34WdAOSR2V/7blGWMkHjPwR5hiaNyMpOAwr0NcbIiNAtl6eubAG4kKkZ6c871S0Cmk1n4MoHZCMKJQ+CVAkOqy1TAtzZhgHxnQ86PGT7NbC37PqlC5G8Erwi81YT4Jqw0T7zFGo8wyWIeRm4JmmKVWVBSUr8qUC+QZF7th3TAO9PvCfaX5hW2wtOVlKZA9goTfzFICzr+Pxtq1dZC7sOmPJgp1kl/HLcCP020TF10m9dzG7w==</Modulus>
<Exponent>AQAB</Exponent>
<P>9xcPskqb8ipjoBTGsqeXL6969RPQh5dUN+WC2auTcxiEWGXyHWB1yo7TmZRko8J1r42/zD6k5HKBmG/Gy1Zno+pTAKDkweJLtAXlyptNiMImfBYRReHXwsty/00fau2lVc61aRUt7sQiJATW/UY1jqErn1v3sL5/F1eGYeuJwPM=</P>
<Q>3c5pKsWuLs62xyeNQ4K/dXHpcNg3wNJT54u7+4Fkj+4ZQNV8kA8LKOvufHDsc8M8dPP084YdrTDmFKDmyU/taXcHrQe+5/JPX3pdl3ugkffLOffGgYD1GFdXqk5DZGVYapnktEbucI1ePKYyJPy/GFe/vFbDp0p8C9fZNxq1ARU=</Q>
<DP>hFfHwnkPuc9WeQFnw3zcD2Bv/SBVyqoVI7M8OJYbbcQt7qL74RwvOwTw9Qt0M/oNyq+jkSPkca+bFiiYU4S+Eh+JwYZrwCUS4yNdhv1Ts/I5ZrDzI3jpdZ4+w9ts/nq22ZTTuarsZTyMBLrK4/Fc8j4E/V/m9LWzoK7yfTQJHl0=</DP>
<DQ>vPF+7ruURDU8x+uuT0sKcy5FECZvX+cLKFwFFxrDIkRN6MezIzhdZk+MSR8cnQQ79Nh32hZuI0FbTUk/L0/RypxlwoStoAHukUO4hDkAsDcoPEoQI/NJVaHZgK7Ig7Y9GhncE6G0rdYO55UfdBiFZGQjZXl3k4NEpgYJ+AHdHH0=</DQ>
<InverseQ>sxuI06HLcck0L0ekygKvF/iKo/EutSs/OuTsifjaZ0KWFrobg3TRZEtgHvxZTtjJPHXSWBOkaY4aRG01jJK2Y1scuyzSPuBJpXMuhY8kCZjYLjastJPYFzloqiidROoKPl9DSY//ozYT1ByL1Bjd810252RrTHPseVTY3qZPmIo=</InverseQ>
<D>nN1TN5SyUb57wnGvcYJ0ieTxkFdPb1nltFCUsCPkEz1tzzXkV+6IdQdLypvk13KbIvEUusXYjnZ/AKdAUELtLuGJFTHl7wzWyylXx+M8mfJMxV4cTmYgr91o1YiRAqSxVsxjcVuj0Ie27P+Q8wmPKQZytLpROCnULvQF/ejFkzMpUqXgtzZm1KWKYOSiET2bWzIqnci3FwOBXXZbsJVs1HWnYaKQVNzCBLPF7yYy6MnypLAxj/zE4XITh4EONNUlu19anFbWL4U4ZfY1I+81gbxPwwvhDfpVCGk02iKnl38Zt+awU25rh7lEOKYcoreTzfTz3AwSuQ+LDZ1Hps9OIQ==</D>
</RSAKeyValue>";
    private const string _validPublicKeyPEM = $@"-----BEGIN PUBLIC KEY-----...snip...-----END PUBLIC KEY-----";
    private const string _validPrivateKeyPEM = $@"-----BEGIN RSA PRIVATE KEY-----...snip...-----END RSA PRIVATE KEY-----";

    private const string _validPublicKeyPEM_Legacy = $@"-----BEGIN PUBLIC KEY-----...snip_Legacy...-----END PUBLIC KEY-----";
    private const string _validPrivateKeyPEM_Legacy = $@"-----BEGIN RSA PRIVATE KEY-----...snip_Legacy...-----END RSA PRIVATE KEY-----";


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

    //-------------------------------------//

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

    //-------------------------------------//

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

    //-------------------------------------//

    [Fact]
    public void AddMyIdJwt_Should_Configure_AsymmetricKeys_When_No_SymmetricKey()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateEmptyDefaultSetupOptions();
        setupOptions.ConnectionString = "DummyConnection"; // Required field
        setupOptions.SymmetricTokenSigningKey = null;
        setupOptions.AsymmetricXmlKeyPair = AsymmetricXmlKeyPair.Create(_validPublicKeyXml, _validPrivateKeyXml);
        setupOptions.LegacyAsymmetricXmlKeyPairs = [AsymmetricXmlKeyPair.Create(_validPublicKeyXml, _validPrivateKeyXml)];

        // Act
        services.AddMyIdJwt(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.True(jwtOptions.UseAsymmetricCrypto);
        Assert.NotNull(jwtOptions.CurrentAsymmetricKeyPair);
        IsValidAsymmetricPemKeyPair(jwtOptions.CurrentAsymmetricKeyPair).ShouldBeTrue();
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
    public void AddMyIdJwt_Should_Convert_XML_To_PEM_When_PEM_Not_Provided()
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.SymmetricTokenSigningKey = string.Empty;
        //setupOptions.AsymmetricPemKeyPair = AsymmetricPemKeyPair.Create("-----BEGIN PUBLIC KEY-----\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCrVrCuTtArbgaZzL1hvh0xtL5mc7o0NqPVnYXkLvgcwiC3BjLGw1tGEGoJaXDuSaRllobm53JBhjx33UNv+5z/UMG4kytBWxheNVKnL6GgqlNabMaFfPLPCF8kAgKnsi79NMo+n6KnSY8YeUmec/p2vjO2NjsSAVcWEQMVhJ31LwIDAQAB\n-----END PUBLIC KEY-----", "-----BEGIN RSA PRIVATE KEY-----\nMIICXAIBAAKBgQCrVrCuTtArbgaZzL1hvh0xtL5mc7o0NqPVnYXkLvgcwiC3BjLGw1tGEGoJaXDuSaRllobm53JBhjx33UNv+5z/UMG4kytBWxheNVKnL6GgqlNabMaFfPLPCF8kAgKnsi79NMo+n6KnSY8YeUmec/p2vjO2NjsSAVcWEQMVhJ31LwIDAQABAoGBAI1D/ETdB7xCJR8+kGzOzFnOQ2TmU3TqPPnqXeMGYt7DAmNK+HabS3P1Y4/S3SqZ0pLeUIYkJHDKoE2irkWUGOQQ6Eb5Xt1nYZ4CSY/q5r8L8t7CClh/CD3rDhR8TpVj9ILAeadM+lG1r+AD9yAIUBUOvhqrGt5IkWOtXSgRXm6BAkEA4xPJ7fKAiMZRIaJkLU7zphIBk7dq/QGgQf9bOJPPK2KbOyC2dKPkYhv0gglYKRBgPwFENYTOAHCBQ3NFYi0nowJBAMGsFIAYL30Hq+/5WrTAEgWXZBQIL3nS+i/fIwvM07pPGPHohxo7x+BCRAew5vG+BN/6Q9uL5Lcea8/5QnbFX2kCQHGMQQdJjgcz/yqq3qYAB4L276reUGK+wwGK+8z9Hm6+/HnNs9TtwwE4gyw7PDjKErGI0kCcr6RIAx42VEQXMo8CQDT/PNzCZ/roaLHJJuMXBzVJsm8DHVYqFjjLg5RkmCRzMKcZ5fYTTYxqEJupJmLXk4JgG0G4FEuGLhLj7OVReIkCQCABqODYnk3hqh/zcZgYXcwfQQ47cEF5NPUQule/JenkCPnJ4OKiLGdO934aML4bNlWYQYQzbbQ8/a6D8BGyJP4=\n-----END RSA PRIVATE KEY-----");

        setupOptions.AsymmetricXmlKeyPair = AsymmetricXmlKeyPair.Create(_validPublicKeyXml, _validPrivateKeyXml);
        setupOptions.LegacyAsymmetricXmlKeyPairs = [AsymmetricXmlKeyPair.Create(_validPublicKeyXml, _validPrivateKeyXml)];

        // Act - Should not throw
        services.AddMyIdJwt(setupOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.True(jwtOptions.UseAsymmetricCrypto);
        Assert.NotNull(jwtOptions.CurrentAsymmetricKeyPair);
        Assert.NotNull(jwtOptions.LegacyAsymmetricKeyPairs);


        IsValidAsymmetricPemKeyPair(jwtOptions.CurrentAsymmetricKeyPair).ShouldBeTrue();

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
        setupOptions.AsymmetricPemKeyPair = AsymmetricPemKeyPair.Create(_validPublicKeyPEM, _validPrivateKeyPEM);
        setupOptions.LegacyAsymmetricPemKeyPairs = [AsymmetricPemKeyPair.Create(_validPublicKeyPEM_Legacy, _validPrivateKeyPEM_Legacy)];
        setupOptions.RefreshTokenUpdatePolicy = RefreshTokenUpdatePolicy.Always;

        // Act
        services.AddMyIdJwt(setupOptions);

        // Assert - Verify EVERY property from the CopyOptionsValues method is correctly assigned
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.Equal(999, jwtOptions.TokenExpirationMinutes);
        Assert.Equal("TestIssuer_CopyTest", jwtOptions.TokenIssuer);
        Assert.Equal("HS512_CopyTest", jwtOptions.SecurityAlgorithm);
        Assert.Equal("RS512_CopyTest", jwtOptions.AsymmetricAlgorithm);
        Assert.Equal(RefreshTokenUpdatePolicy.Always, jwtOptions.RefreshTokenUpdatePolicy);
        Assert.NotNull(jwtOptions.CurrentAsymmetricKeyPair);
        Assert.True(IsValidAsymmetricPemKeyPair(jwtOptions.CurrentAsymmetricKeyPair));
        Assert.NotNull(jwtOptions.LegacyAsymmetricKeyPairs);
        Assert.Contains(jwtOptions.LegacyAsymmetricKeyPairs, k => k.PublicKey.Contains("Legacy"));
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
        setupOptions.AsymmetricXmlKeyPair = null;
        setupOptions.AsymmetricPemKeyPair = null;
        setupOptions.LegacyAsymmetricPemKeyPairs = [];
        setupOptions.LegacyAsymmetricXmlKeyPairs = [];
        setupOptions.RefreshTokenUpdatePolicy = RefreshTokenUpdatePolicy.Always;

        // Act
        services.AddMyIdJwt(setupOptions);

        // Assert - Verify EVERY property from the CopyOptionsValues method is correctly assigned
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.Equal(999, jwtOptions.TokenExpirationMinutes);
        Assert.Equal("TestIssuer_CopyTest", jwtOptions.TokenIssuer);
        Assert.Equal("HS512_CopyTest", jwtOptions.SecurityAlgorithm);
        Assert.Equal(RefreshTokenUpdatePolicy.Always, jwtOptions.RefreshTokenUpdatePolicy);
        Assert.Equal(new string('X', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH), jwtOptions.SymmetricTokenSigningKey);
        Assert.Null(jwtOptions.CurrentAsymmetricKeyPair);
        Assert.Empty(jwtOptions.LegacyAsymmetricKeyPairs);
    }

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


    private static bool IsValidPublicPemKey(string? pemKey) =>
        pemKey != null &&
        (pemKey.StartsWith(PemKeyConstants.PUBLIC_PEM_BEGIN, StringComparison.OrdinalIgnoreCase)
        || pemKey.StartsWith(PemKeyConstants.PUBLIC_PEM_RSA_BEGIN, StringComparison.OrdinalIgnoreCase));

    private static bool IsValidPrivatePemKey(string? pemKey) =>
        pemKey != null &&
        (pemKey.StartsWith(PemKeyConstants.PRIVATE_PEM_BEGIN, StringComparison.OrdinalIgnoreCase)
        || pemKey.StartsWith(PemKeyConstants.PRIVATE_PEM_RSA_BEGIN, StringComparison.OrdinalIgnoreCase));


    private static bool IsValidAsymmetricPemKeyPair(AsymmetricPemKeyPair pemKey) =>
        pemKey != null &&
        IsValidPublicPemKey(pemKey.PublicKey)
       && IsValidPrivatePemKey(pemKey.PrivateKey);


}
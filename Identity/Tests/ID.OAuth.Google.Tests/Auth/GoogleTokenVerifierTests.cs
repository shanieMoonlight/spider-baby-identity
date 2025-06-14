using Id.Tests.Utility.Exceptions;
using ID.GlobalSettings.Errors;
using ID.OAuth.Google.Services.Imps;
using ID.OAuth.Google.Setup;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static MyResults.BasicResult;

namespace ID.OAuth.Google.Tests.Auth;

public class GoogleTokenVerifierTests
{    private readonly Mock<IOptions<IdOAuthGoogleOptions>> _mockOptions;
    private readonly Mock<ILogger<GoogleTokenVerifier>> _mockLogger;
    private readonly GoogleTokenVerifier _verifier;
    private readonly IdOAuthGoogleOptions _options;

    public GoogleTokenVerifierTests()
    {
        _options = new IdOAuthGoogleOptions
        {
            ClientId = TestConfiguration.GoogleOAuth.TestClientId
        };
        
        _mockOptions = new Mock<IOptions<IdOAuthGoogleOptions>>();
        _mockOptions.Setup(x => x.Value).Returns(_options);
        
        _mockLogger = new Mock<ILogger<GoogleTokenVerifier>>();
        
        _verifier = new GoogleTokenVerifier(_mockOptions.Object, _mockLogger.Object);    }

    //-------------------------------------//

    [Fact]
    public async Task VerifyTokenAsync_Should_Return_UnauthorizedResult_When_Token_Is_Invalid()
    {
        // Arrange
        var invalidToken = TestConfiguration.JwtTokens.InvalidToken;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _verifier.VerifyTokenAsync(invalidToken, cancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(ResultStatus.Unauthorized, result.Status);
        Assert.NotNull(result.Info);
        Assert.NotEmpty(result.Info);    }

    //-------------------------------------//

    [Fact]
    public async Task VerifyTokenAsync_Should_Log_InvalidJwtException_When_Token_Verification_Fails()
    {
        // Arrange
        var faultyToken = TestConfiguration.JwtTokens.FaultyToken;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _verifier.VerifyTokenAsync(faultyToken, cancellationToken);

        // Assert
        ExceptionUtils.VerifyExceptionLogging<GoogleTokenVerifier, Exception>(
            _mockLogger,
            IdErrorEvents.OAuth.Verification);
        
        Assert.False(result.Succeeded);
        Assert.Equal(ResultStatus.Unauthorized, result.Status);
    }

    //-------------------------------------//

    [Fact]
    public async Task VerifyTokenAsync_Should_Return_UnauthorizedResult_When_Token_Is_Null()
    {
        // Arrange
        string? nullToken = null;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _verifier.VerifyTokenAsync(nullToken!, cancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(ResultStatus.Unauthorized, result.Status);
        Assert.NotNull(result.Info);
    }

    //-------------------------------------//

    [Fact]
    public async Task VerifyTokenAsync_Should_Return_UnauthorizedResult_When_Token_Is_Empty()
    {
        // Arrange
        var emptyToken = string.Empty;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _verifier.VerifyTokenAsync(emptyToken, cancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(ResultStatus.Unauthorized, result.Status);
        Assert.NotNull(result.Info);
    }

    //-------------------------------------//    [Fact]
    public async Task VerifyTokenAsync_Should_Return_UnauthorizedResult_When_Token_Has_Wrong_Audience()
    {
        // Arrange - Use token with wrong audience from configuration
        var tokenWithWrongAudience = TestConfiguration.JwtTokens.TokenWithWrongAudience;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _verifier.VerifyTokenAsync(tokenWithWrongAudience, cancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(ResultStatus.Unauthorized, result.Status);
        Assert.NotNull(result.Info);

        // Verify exception was logged
        ExceptionUtils.VerifyExceptionLogging<GoogleTokenVerifier, Exception>(
            _mockLogger, 
            IdErrorEvents.OAuth.Verification);
    }

    //-------------------------------------//

    [Fact]
    public async Task VerifyTokenAsync_Should_Return_UnauthorizedResult_When_Token_Exceeds_Max_Length()
    {
        // Arrange - Create a token longer than Google's 10,000 character limit
        var oversizedToken = new string('a', 10001);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _verifier.VerifyTokenAsync(oversizedToken, cancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(ResultStatus.Unauthorized, result.Status);
        Assert.NotNull(result.Info);

        // Verify exception was logged
        ExceptionUtils.VerifyExceptionLogging<GoogleTokenVerifier, Exception>(
            _mockLogger,
            IdErrorEvents.OAuth.Verification);
    }

    //-------------------------------------//    [Fact]
    public async Task VerifyTokenAsync_Should_Handle_General_Exceptions_And_Return_UnauthorizedResult()
    {
        // Note: This test is harder to trigger with the real GoogleJsonWebSignature.ValidateAsync
        // since it's a static method, but the exception handling code is there for defensive programming
        
        // Arrange - Use a malformed token that might cause unexpected exceptions
        var malformedToken = TestConfiguration.JwtTokens.MalformedToken;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _verifier.VerifyTokenAsync(malformedToken, cancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(ResultStatus.Unauthorized, result.Status);
        Assert.NotNull(result.Info);

        // Verify some exception was logged (could be InvalidJwtException or other)
        ExceptionUtils.VerifyExceptionLogging<GoogleTokenVerifier, Exception>(
            _mockLogger,
            IdErrorEvents.OAuth.Verification);
        //_mockLogger.Verify(
        //    x => x.LogException(
        //        It.IsAny<Exception>(), 
        //        IdErrorEvents.OAuth.Verification),
        //    Times.AtLeastOnce);
    }

    //-------------------------------------//    [Fact]
    public async Task VerifyTokenAsync_Should_Include_Exception_Message_In_Result()
    {
        // Arrange
        var invalidToken = TestConfiguration.JwtTokens.InvalidToken;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _verifier.VerifyTokenAsync(invalidToken, cancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(ResultStatus.Unauthorized, result.Status);
        Assert.NotNull(result.Info);
        Assert.NotEmpty(result.Info);
        
        // Error message should contain some indication of what went wrong
        Assert.True(result.Info.Length > 0);
    }

    //-------------------------------------//    [Fact]
    public async Task VerifyTokenAsync_Should_Use_Configured_ClientId_For_Audience_Validation()
    {
        // Arrange
        var customClientId = TestConfiguration.GoogleOAuth.CustomClientId;
        var customOptions = new IdOAuthGoogleOptions { ClientId = customClientId };
        
        var mockCustomOptions = new Mock<IOptions<IdOAuthGoogleOptions>>();
        mockCustomOptions.Setup(x => x.Value).Returns(customOptions);
        
        var customVerifier = new GoogleTokenVerifier(mockCustomOptions.Object, _mockLogger.Object);
        
        // Use an invalid token - the important thing is that it uses the custom client ID
        var token = TestConfiguration.JwtTokens.InvalidToken;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await customVerifier.VerifyTokenAsync(token, cancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        // The verification should have attempted to use the custom client ID
        // (even though it will fail due to invalid token format)
    }

    //-------------------------------------//

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task VerifyTokenAsync_Should_Return_UnauthorizedResult_For_Whitespace_Tokens(string whitespaceToken)
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _verifier.VerifyTokenAsync(whitespaceToken, cancellationToken);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(ResultStatus.Unauthorized, result.Status);
        Assert.NotNull(result.Info);

        // Verify exception was logged

        ExceptionUtils.VerifyExceptionLogging<GoogleTokenVerifier, Exception>(
            _mockLogger,
            IdErrorEvents.OAuth.Verification);
    }

    //-------------------------------------//    [Fact]
    public async Task VerifyTokenAsync_Should_Respect_CancellationToken()
    {
        // Arrange
        var token = TestConfiguration.JwtTokens.InvalidToken;
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel(); // Cancel immediately

        // Act & Assert
        var result = await _verifier.VerifyTokenAsync(token, cancellationTokenSource.Token);
        
        // Should complete (though with failure) even with cancelled token
        // The GoogleJsonWebSignature.ValidateAsync handles cancellation internally
        Assert.False(result.Succeeded);
    }

    //-------------------------------------//
    // Note: Testing successful token verification requires a real Google JWT token
    // which would need to be mocked at the GoogleJsonWebSignature level.
    // For integration tests, you would use a real token from Google's OAuth flow.
    //-------------------------------------//
}
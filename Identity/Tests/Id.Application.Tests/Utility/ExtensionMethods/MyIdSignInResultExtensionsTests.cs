using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Domain.Models;

namespace ID.Application.Tests.Utility.ExtensionMethods;

public class MyIdSignInResultExtensionsTests
{
    #region ToGenResult Tests

    [Fact]
    public void ToGenResult_WhenNotFound_ReturnsNotFoundGenResult()
    {
        // Arrange
        var signInResult = MyIdSignInResult.NotFoundResult();
        var testValue = "TestValue";

        // Act
        var result = signInResult.ToGenResult(testValue);

        // Assert
        Assert.True(result.NotFound);
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
        Assert.Null(result.Value);
    }

    [Fact]
    public void ToGenResult_WhenUnauthorized_ReturnsUnauthorizedGenResult()
    {
        // Arrange
        var signInResult = MyIdSignInResult.UnauthorizedResult();
        var testValue = "TestValue";

        // Act
        var result = signInResult.ToGenResult(testValue);

        // Assert
        Assert.True(result.Unauthorized);
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
        Assert.Null(result.Value);
    }

    [Fact]
    public void ToGenResult_WhenEmailConfirmationRequired_ReturnsPreconditionRequiredGenResult()
    {
        // Arrange
        var email = "test@example.com";
        var signInResult = MyIdSignInResult.EmailConfirmedRequiredResult(email);
        var testValue = "TestValue";

        // Act
        var result = signInResult.ToGenResult(testValue);

        // Assert
        Assert.True(result.PreconditionRequired);
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
        Assert.Equal(testValue, result.Value); // The value is preserved for PreconditionRequired
    }

    [Fact]
    public void ToGenResult_WhenTwoFactorRequired_ReturnsPreconditionRequiredGenResult()
    {
        // Arrange - Use data factories instead of direct instantiation
        var user = AppUserDataFactory.CreateNoTeam();
        var team = TeamDataFactory.AnyTeam;
        var mfaData = MfaResultData.Create(TwoFactorProvider.Email);
        
        var signInResult = MyIdSignInResult.TwoFactorRequiredResult(mfaData, user, team);
        var testValue = "TestValue";

        // Act
        var result = signInResult.ToGenResult(testValue);

        // Assert
        Assert.True(result.PreconditionRequired);
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
        Assert.Equal(testValue, result.Value); // The value is preserved for PreconditionRequired
    }

    [Fact]
    public void ToGenResult_WhenGeneralFailure_ReturnsFailureGenResult()
    {
        // Arrange
        var failureMessage = "General failure";
        var signInResult = MyIdSignInResult.Failure(failureMessage);
        var testValue = "TestValue";

        // Act
        var result = signInResult.ToGenResult(testValue);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
        Assert.Null(result.Value);
    }

    [Fact]
    public void ToGenResult_WhenSuccess_ReturnsSuccessGenResult()
    {
        // Arrange - Use data factories instead of direct instantiation
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        
        var signInResult = MyIdSignInResult.Success(user, team);
        var testValue = "TestValue";

        // Act
        var result = signInResult.ToGenResult(testValue);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
        Assert.Equal(testValue, result.Value);
    }

    #endregion

    #region ToGenResultFailure Tests

    [Fact]
    public void ToGenResultFailure_WhenEmailConfirmationRequired_ReturnsPreconditionRequiredGenResult()
    {
        // Arrange
        var email = "test@example.com";
        var signInResult = MyIdSignInResult.EmailConfirmedRequiredResult(email);

        // Act
        var result = signInResult.ToGenResultFailure<string>();

        // Assert
        Assert.True(result.PreconditionRequired);
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
        Assert.Null(result.Value);
    }

    [Fact]
    public void ToGenResultFailure_WhenTwoFactorRequired_ReturnsPreconditionRequiredGenResult()
    {
        // Arrange - Use data factories instead of direct instantiation
        var user = AppUserDataFactory.CreateNoTeam();
        var team = TeamDataFactory.AnyTeam;
        var mfaData = MfaResultData.Create(TwoFactorProvider.Email);
        
        var signInResult = MyIdSignInResult.TwoFactorRequiredResult(mfaData, user, team);

        // Act
        var result = signInResult.ToGenResultFailure<string>();

        // Assert
        Assert.True(result.PreconditionRequired);
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
        Assert.Null(result.Value);
    }

    [Fact]
    public void ToGenResultFailure_WhenNotFound_ReturnsNotFoundGenResult()
    {
        // Arrange
        var signInResult = MyIdSignInResult.NotFoundResult();

        // Act
        var result = signInResult.ToGenResultFailure<string>();

        // Assert
        Assert.True(result.NotFound);
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
        Assert.Null(result.Value);
    }

    [Fact]
    public void ToGenResultFailure_WhenUnauthorized_ReturnsUnauthorizedGenResult()
    {
        // Arrange
        var signInResult = MyIdSignInResult.UnauthorizedResult();

        // Act
        var result = signInResult.ToGenResultFailure<string>();

        // Assert
        Assert.True(result.Unauthorized);
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
        Assert.Null(result.Value);
    }

    [Fact]
    public void ToGenResultFailure_WhenGeneralFailure_ReturnsFailureGenResult()
    {
        // Arrange
        var failureMessage = "General failure";
        var signInResult = MyIdSignInResult.Failure(failureMessage);

        // Act
        var result = signInResult.ToGenResultFailure<string>();

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
        Assert.Null(result.Value);
    }

    #endregion

    #region ToBasicResult Tests

    [Fact]
    public void ToBasicResult_WhenNotFound_ReturnsNotFoundBasicResult()
    {
        // Arrange
        var signInResult = MyIdSignInResult.NotFoundResult();

        // Act
        var result = signInResult.ToBasicResult();

        // Assert
        Assert.True(result.NotFound);
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
    }

    [Fact]
    public void ToBasicResult_WhenUnauthorized_ReturnsUnauthorizedBasicResult()
    {
        // Arrange
        var signInResult = MyIdSignInResult.UnauthorizedResult();

        // Act
        var result = signInResult.ToBasicResult();

        // Assert
        Assert.True(result.Unauthorized);
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
    }

    [Fact]
    public void ToBasicResult_WhenEmailConfirmationRequired_ReturnsPreconditionRequiredBasicResult()
    {
        // Arrange
        var email = "test@example.com";
        var signInResult = MyIdSignInResult.EmailConfirmedRequiredResult(email);

        // Act
        var result = signInResult.ToBasicResult();

        // Assert
        Assert.True(result.PreconditionRequired);
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
    }

    [Fact]
    public void ToBasicResult_WhenTwoFactorRequired_ReturnsPreconditionRequiredBasicResult()
    {
        // Arrange - Use data factories instead of direct instantiation
        var user = AppUserDataFactory.CreateNoTeam();
        var team = TeamDataFactory.AnyTeam;
        var mfaData = MfaResultData.Create(TwoFactorProvider.Email);
        
        var signInResult = MyIdSignInResult.TwoFactorRequiredResult(mfaData, user, team);

        // Act
        var result = signInResult.ToBasicResult();

        // Assert
        Assert.True(result.PreconditionRequired);
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
    }

    [Fact]
    public void ToBasicResult_WhenGeneralFailure_ReturnsFailureBasicResult()
    {
        // Arrange
        var failureMessage = "General failure";
        var signInResult = MyIdSignInResult.Failure(failureMessage);

        // Act
        var result = signInResult.ToBasicResult();

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
    }

    [Fact]
    public void ToBasicResult_WhenSuccess_ReturnsSuccessBasicResult()
    {
        // Arrange - Use data factories instead of direct instantiation
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        
        var signInResult = MyIdSignInResult.Success(user, team);

        // Act
        var result = signInResult.ToBasicResult();

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(signInResult.Message, result.Info);
    }

    #endregion
}
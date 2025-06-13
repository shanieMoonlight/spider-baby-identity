//using ID.Application.AppImps.RequestInfo;
//using ID.Domain.Claims.Utils;
//using Microsoft.AspNetCore.Http;
//using Moq;
//using Shouldly;
//using System.Security.Claims;
//using Xunit;

//namespace ID.Application.Tests.AppImps.RequestInfo;

//public class UserInfoTests
//{
//    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
//    private readonly Mock<HttpContext> _httpContextMock;

//    public UserInfoTests()
//    {
//        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
//        _httpContextMock = new Mock<HttpContext>();
//    }

//    private UserInfo CreateUserInfo(ClaimsPrincipal? principal = null)
//    {
//        _httpContextMock.Setup(x => x.User).Returns(principal ?? new ClaimsPrincipal());
//        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);
//        return new UserInfo(_httpContextAccessorMock.Object);
//    }

//    private ClaimsPrincipal CreatePrincipalWithClaims(params Claim[] claims)
//    {
//        var identity = new ClaimsIdentity(claims, "test");
//        return new ClaimsPrincipal(identity);
//    }

//    #region GetLoggedInUserId Tests

//    [Fact]
//    public void GetLoggedInUserId_WithNameIdentifierClaim_ShouldReturnUserId()
//    {
//        // Arrange
//        var userId = "12345";
//        var principal = CreatePrincipalWithClaims(new Claim(ClaimTypes.NameIdentifier, userId));
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserId();

//        // Assert
//        result.ShouldBe(userId);
//    }

//    [Fact]
//    public void GetLoggedInUserId_WithNoNameIdentifierClaim_ShouldReturnSystem()
//    {
//        // Arrange
//        var principal = CreatePrincipalWithClaims();
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserId();

//        // Assert
//        result.ShouldBe("SYSTEM");
//    }

//    [Fact]
//    public void GetLoggedInUserId_WithFallback_ShouldReturnFallback()
//    {
//        // Arrange
//        var fallback = "FALLBACK_USER";
//        var principal = CreatePrincipalWithClaims();
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserId(fallback);

//        // Assert
//        result.ShouldBe(fallback);
//    }

//    [Fact]
//    public void GetLoggedInUserId_WithNullHttpContext_ShouldReturnSystem()
//    {
//        // Arrange
//        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
//        var userInfo = new UserInfo(_httpContextAccessorMock.Object);

//        // Act
//        var result = userInfo.GetLoggedInUserId();

//        // Assert
//        result.ShouldBe("SYSTEM");
//    }

//    [Fact]
//    public void GetLoggedInUserId_WithNullHttpContextAccessor_ShouldReturnSystem()
//    {
//        // Arrange
//        var userInfo = new UserInfo(null!);

//        // Act
//        var result = userInfo.GetLoggedInUserId();

//        // Assert
//        result.ShouldBe("SYSTEM");
//    }

//    #endregion

//    #region GetLoggedInUserName Tests

//    [Fact]
//    public void GetLoggedInUserName_WithMyIdNameClaim_ShouldReturnUserName()
//    {
//        // Arrange
//        var userName = "john.doe";
//        var principal = CreatePrincipalWithClaims(new Claim(MyIdClaimTypes.NAME, userName));
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserName();

//        // Assert
//        result.ShouldBe(userName);
//    }

//    [Fact]
//    public void GetLoggedInUserName_WithStandardNameClaim_ShouldReturnUserName()
//    {
//        // Arrange
//        var userName = "john.doe";
//        var principal = CreatePrincipalWithClaims(new Claim(ClaimTypes.Name, userName));
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserName();

//        // Assert
//        result.ShouldBe(userName);
//    }

//    [Fact]
//    public void GetLoggedInUserName_WithBothNameClaims_ShouldPreferMyIdName()
//    {
//        // Arrange
//        var myIdName = "custom.name";
//        var standardName = "standard.name";
//        var principal = CreatePrincipalWithClaims(
//            new Claim(MyIdClaimTypes.NAME, myIdName),
//            new Claim(ClaimTypes.Name, standardName));
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserName();

//        // Assert
//        result.ShouldBe(myIdName);
//    }

//    [Fact]
//    public void GetLoggedInUserName_WithNoNameClaim_ShouldReturnSystem()
//    {
//        // Arrange
//        var principal = CreatePrincipalWithClaims();
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserName();

//        // Assert
//        result.ShouldBe("SYSTEM");
//    }

//    [Fact]
//    public void GetLoggedInUserName_WithFallback_ShouldReturnFallback()
//    {
//        // Arrange
//        var fallback = "FALLBACK_NAME";
//        var principal = CreatePrincipalWithClaims();
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserName(fallback);

//        // Assert
//        result.ShouldBe(fallback);
//    }

//    #endregion

//    #region GetLoggedInUserEmail Tests

//    [Fact]
//    public void GetLoggedInUserEmail_WithMyIdEmailClaim_ShouldReturnEmail()
//    {
//        // Arrange
//        var email = "john@example.com";
//        var principal = CreatePrincipalWithClaims(new Claim(MyIdClaimTypes.EMAIL, email));
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserEmail();

//        // Assert
//        result.ShouldBe(email);
//    }

//    [Fact]
//    public void GetLoggedInUserEmail_WithStandardEmailClaim_ShouldReturnEmail()
//    {
//        // Arrange
//        var email = "john@example.com";
//        var principal = CreatePrincipalWithClaims(new Claim(ClaimTypes.Email, email));
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserEmail();

//        // Assert
//        result.ShouldBe(email);
//    }

//    [Fact]
//    public void GetLoggedInUserEmail_WithBothEmailClaims_ShouldPreferMyIdEmail()
//    {
//        // Arrange
//        var myIdEmail = "custom@example.com";
//        var standardEmail = "standard@example.com";
//        var principal = CreatePrincipalWithClaims(
//            new Claim(MyIdClaimTypes.EMAIL, myIdEmail),
//            new Claim(ClaimTypes.Email, standardEmail));
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserEmail();

//        // Assert
//        result.ShouldBe(myIdEmail);
//    }

//    [Fact]
//    public void GetLoggedInUserEmail_WithNoEmailClaim_ShouldReturnSystem()
//    {
//        // Arrange
//        var principal = CreatePrincipalWithClaims();
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserEmail();

//        // Assert
//        result.ShouldBe("SYSTEM");
//    }

//    [Fact]
//    public void GetLoggedInUserEmail_WithFallback_ShouldReturnFallback()
//    {
//        // Arrange
//        var fallback = "fallback@example.com";
//        var principal = CreatePrincipalWithClaims();
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserEmail(fallback);

//        // Assert
//        result.ShouldBe(fallback);
//    }

//    #endregion

//    #region GetLoggedInNameAndEmail Tests

//    [Fact]
//    public void GetLoggedInNameAndEmail_WithNameAndEmail_ShouldReturnCombined()
//    {
//        // Arrange
//        var name = "John Doe";
//        var email = "john@example.com";
//        var principal = CreatePrincipalWithClaims(
//            new Claim(MyIdClaimTypes.NAME, name),
//            new Claim(MyIdClaimTypes.EMAIL, email));
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInNameAndEmail();

//        // Assert
//        result.ShouldBe($"{name} / {email}");
//    }

//    [Fact]
//    public void GetLoggedInNameAndEmail_WithNoClaims_ShouldReturnSystemSystem()
//    {
//        // Arrange
//        var principal = CreatePrincipalWithClaims();
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInNameAndEmail();

//        // Assert
//        result.ShouldBe("SYSTEM / SYSTEM");
//    }

//    [Fact]
//    public void GetLoggedInNameAndEmail_WithFallback_ShouldUseFallbackForBoth()
//    {
//        // Arrange
//        var fallback = "FALLBACK";
//        var principal = CreatePrincipalWithClaims();
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInNameAndEmail(fallback);

//        // Assert
//        result.ShouldBe($"{fallback} / {fallback}");
//    }

//    [Fact]
//    public void GetLoggedInNameAndEmail_WithOnlyName_ShouldReturnNameAndSystem()
//    {
//        // Arrange
//        var name = "John Doe";
//        var principal = CreatePrincipalWithClaims(new Claim(MyIdClaimTypes.NAME, name));
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInNameAndEmail();

//        // Assert
//        result.ShouldBe($"{name} / SYSTEM");
//    }

//    [Fact]
//    public void GetLoggedInNameAndEmail_WithOnlyEmail_ShouldReturnSystemAndEmail()
//    {
//        // Arrange
//        var email = "john@example.com";
//        var principal = CreatePrincipalWithClaims(new Claim(MyIdClaimTypes.EMAIL, email));
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInNameAndEmail();

//        // Assert
//        result.ShouldBe($"SYSTEM / {email}");
//    }

//    #endregion

//    #region Edge Cases and Complex Scenarios

//    [Fact]
//    public void GetLoggedInUserName_WithEmptyStringClaim_ShouldReturnEmptyString()
//    {
//        // Arrange
//        var principal = CreatePrincipalWithClaims(new Claim(MyIdClaimTypes.NAME, ""));
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserName();

//        // Assert
//        result.ShouldBe("");
//    }

//    [Fact]
//    public void GetLoggedInUserEmail_WithEmptyStringClaim_ShouldReturnEmptyString()
//    {
//        // Arrange
//        var principal = CreatePrincipalWithClaims(new Claim(MyIdClaimTypes.EMAIL, ""));
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserEmail();

//        // Assert
//        result.ShouldBe("");
//    }

//    [Fact]
//    public void GetLoggedInUserId_WithEmptyStringClaim_ShouldReturnEmptyString()
//    {
//        // Arrange
//        var principal = CreatePrincipalWithClaims(new Claim(ClaimTypes.NameIdentifier, ""));
//        var userInfo = CreateUserInfo(principal);

//        // Act
//        var result = userInfo.GetLoggedInUserId();

//        // Assert
//        result.ShouldBe("");
//    }

//    [Theory]
//    [InlineData("user123", "John Doe", "john@example.com", "John Doe / john@example.com")]
//    [InlineData("", "", "", " / ")]
//    [InlineData("admin", "Administrator", "admin@system.com", "Administrator / admin@system.com")]
//    [InlineData("test", "Test User", "test@test.org", "Test User / test@test.org")]
//    public void AllMethods_WithVariousInputs_ShouldReturnExpectedValues(
//        string userId, string userName, string userEmail, string expectedNameAndEmail)
//    {
//        // Arrange
//        var principal = CreatePrincipalWithClaims(
//            new Claim(ClaimTypes.NameIdentifier, userId),
//            new Claim(MyIdClaimTypes.NAME, userName),
//            new Claim(MyIdClaimTypes.EMAIL, userEmail));
//        var userInfo = CreateUserInfo(principal);

//        // Act & Assert
//        userInfo.GetLoggedInUserId().ShouldBe(userId);
//        userInfo.GetLoggedInUserName().ShouldBe(userName);
//        userInfo.GetLoggedInUserEmail().ShouldBe(userEmail);
//        userInfo.GetLoggedInNameAndEmail().ShouldBe(expectedNameAndEmail);
//    }

//    [Fact]
//    public void Methods_WithSpecialCharacters_ShouldHandleCorrectly()
//    {
//        // Arrange
//        var userId = "user@domain.com";
//        var userName = "User (Admin)";
//        var userEmail = "user+admin@example.com";
//        var principal = CreatePrincipalWithClaims(
//            new Claim(ClaimTypes.NameIdentifier, userId),
//            new Claim(MyIdClaimTypes.NAME, userName),
//            new Claim(MyIdClaimTypes.EMAIL, userEmail));
//        var userInfo = CreateUserInfo(principal);

//        // Act & Assert
//        userInfo.GetLoggedInUserId().ShouldBe(userId);
//        userInfo.GetLoggedInUserName().ShouldBe(userName);
//        userInfo.GetLoggedInUserEmail().ShouldBe(userEmail);
//        userInfo.GetLoggedInNameAndEmail().ShouldBe($"{userName} / {userEmail}");
//    }

//    [Fact]
//    public void Methods_WithLongValues_ShouldHandleCorrectly()
//    {
//        // Arrange
//        var longUserId = new string('a', 100);
//        var longUserName = new string('b', 200);
//        var longUserEmail = $"{new string('c', 50)}@{new string('d', 50)}.com";
//        var principal = CreatePrincipalWithClaims(
//            new Claim(ClaimTypes.NameIdentifier, longUserId),
//            new Claim(MyIdClaimTypes.NAME, longUserName),
//            new Claim(MyIdClaimTypes.EMAIL, longUserEmail));
//        var userInfo = CreateUserInfo(principal);

//        // Act & Assert
//        userInfo.GetLoggedInUserId().ShouldBe(longUserId);
//        userInfo.GetLoggedInUserName().ShouldBe(longUserName);
//        userInfo.GetLoggedInUserEmail().ShouldBe(longUserEmail);
//        userInfo.GetLoggedInNameAndEmail().ShouldBe($"{longUserName} / {longUserEmail}");
//    }

//    [Fact]
//    public void Methods_WithNullFallback_ShouldReturnSystem()
//    {
//        // Arrange
//        var principal = CreatePrincipalWithClaims();
//        var userInfo = CreateUserInfo(principal);

//        // Act & Assert
//        userInfo.GetLoggedInUserId(null).ShouldBe("SYSTEM");
//        userInfo.GetLoggedInUserName(null).ShouldBe("SYSTEM");
//        userInfo.GetLoggedInUserEmail(null).ShouldBe("SYSTEM");
//        userInfo.GetLoggedInNameAndEmail(null).ShouldBe("SYSTEM / SYSTEM");
//    }

//    [Fact]
//    public void Methods_WithMultipleClaims_ShouldPickFirstOfEachType()
//    {
//        // Arrange
//        var identity = new ClaimsIdentity(new[]
//        {
//            new Claim(ClaimTypes.NameIdentifier, "first-id"),
//            new Claim(ClaimTypes.NameIdentifier, "second-id"),
//            new Claim(MyIdClaimTypes.NAME, "first-name"),
//            new Claim(MyIdClaimTypes.NAME, "second-name"),
//            new Claim(MyIdClaimTypes.EMAIL, "first@example.com"),
//            new Claim(MyIdClaimTypes.EMAIL, "second@example.com")
//        }, "test");
//        var principal = new ClaimsPrincipal(identity);
//        var userInfo = CreateUserInfo(principal);

//        // Act & Assert
//        userInfo.GetLoggedInUserId().ShouldBe("first-id");
//        userInfo.GetLoggedInUserName().ShouldBe("first-name");
//        userInfo.GetLoggedInUserEmail().ShouldBe("first@example.com");
//    }

//    #endregion
//}

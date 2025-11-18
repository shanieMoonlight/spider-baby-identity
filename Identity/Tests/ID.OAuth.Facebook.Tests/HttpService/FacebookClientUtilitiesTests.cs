using ID.OAuth.Facebook.HttpService.Imps;
using System.Security.Cryptography;
using System.Text;

namespace ID.OAuth.Facebook.Tests.HttpService;

public class FacebookClientUtilitiesTests
{
    [Fact]
    public void GenerateAppSecretProof_ShouldReturnExpectedHmac_ForGivenTokenAndSecret()
    {
        // Arrange
        var opts = Options.Create(new IdOAuthFacebookOptions
        {
            AppId = "123",
            AppSecret = "my_super_secret"
        });

        var utils = new FacebookClientUtilities(opts);

        var userToken = "user_token_example";

        // Act
        var proof = utils.GenerateAppSecretProof(userToken);

        // Compute expected using HMACSHA256
        var key = Encoding.UTF8.GetBytes(opts.Value.AppSecret);
        var tokenBytes = Encoding.UTF8.GetBytes(userToken);
        using var hmac = new HMACSHA256(key);
        var expectedHash = hmac.ComputeHash(tokenBytes);
        var expected = BitConverter.ToString(expectedHash).Replace("-", "").ToLowerInvariant();

        // Assert
        proof.ShouldNotBeNullOrWhiteSpace();
        proof.ShouldBe(expected);
        proof.Length.ShouldBe(64); // 32 bytes hex
    }

}//Cls

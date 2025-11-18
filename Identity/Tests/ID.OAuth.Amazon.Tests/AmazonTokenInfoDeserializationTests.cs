using System.Text.Json.Serialization;

namespace ID.OAuth.Amazon.Tests;

public class AmazonTokenInfoDeserializationTests
{

    private static JsonSerializerOptions CreateJsonOptionsWithConverter()
    {
        var jsonOpts = new JsonSerializerOptions
        {

            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        jsonOpts.Converters.Add(new UnixEpochSecondsJsonConverter());

        return jsonOpts;
    }


    //--------------------------//

    [Fact]
    public void Deserialize_SampleTokenInfoJson_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var sampletokeInfoResponseJson = @"
        {
                ""aud"": ""amzn1.application-oa2-client.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"", 
                ""user_id"": ""amzn1.account.ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"",  
                ""iss"": ""https://www.amazon.com"",
                ""app_id"": ""amzn1.application.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"",
                ""exp"": 995,
                ""iat"": 1633036800
        }";

        var opts = CreateJsonOptionsWithConverter();

        using var doc = JsonDocument.Parse(sampletokeInfoResponseJson);
        var tokenInfoElement = doc.RootElement;


        // Act
        var tokenInfo = JsonSerializer.Deserialize<AmazonTokenInfo>(tokenInfoElement.GetRawText(), opts);

        // Assert
        tokenInfo.ShouldNotBeNull();
        tokenInfo.ClientId?.ShouldContain("amzn1.application-oa2-client");
        tokenInfo.UserId?.ShouldStartWith("amzn1.account");
        tokenInfo.Issuer?.ShouldBe("https://www.amazon.com");
        tokenInfo.AppId?.ShouldContain("amzn1.application");
        tokenInfo.ExpiresIn.ShouldBe(995);
        tokenInfo.ExpiresAt.HasValue.ShouldBeTrue();

        // ExpiresAt should be approximately now + ExpiresIn seconds (allowing a small execution delay)
        var now = DateTimeOffset.UtcNow;
        var secondsUntilExpiry = (tokenInfo.ExpiresAt!.Value - now).TotalSeconds;
        secondsUntilExpiry.ShouldBeGreaterThan(990);
        secondsUntilExpiry.ShouldBeLessThan(1010);
    }

}//Cls

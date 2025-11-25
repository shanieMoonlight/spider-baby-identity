using ID.Domain.Claims.AuthMethods;

namespace ID.Domain.Tests.Claims.AuthMethods;

public class AuthMethodClaimValueExtensionsTests
{
    [Fact]
    public void ToClaimValue_ShouldReturnExpectedString_ForAllEnumValues()
    {
        foreach (AuthMethodRef val in Enum.GetValues(typeof(AuthMethodRef)))
        {
            var claim = val.ToClaimValue();

            switch (val)
            {
                case AuthMethodRef.pwd:
                    claim.ShouldBe(AuthMethodClaimValues.PASSWORD);
                    break;
                case AuthMethodRef.otp:
                    claim.ShouldBe(AuthMethodClaimValues.OTP);
                    break;
                case AuthMethodRef.oauth:
                    claim.ShouldBe(AuthMethodClaimValues.OAUTH);
                    break;
                case AuthMethodRef.mfa:
                    claim.ShouldBe(AuthMethodClaimValues.MULTI_FACTOR);
                    break;
                case AuthMethodRef.windowsIntegratedAuth:
                    claim.ShouldBe(AuthMethodClaimValues.WINDOWS_INTEGRATED_AUTH);
                    break;
                case AuthMethodRef.rsa:
                    claim.ShouldBe(AuthMethodClaimValues.RSA);
                    break;
                case AuthMethodRef.fed:
                    claim.ShouldBe(AuthMethodClaimValues.FEDERATED);
                    break;
                case AuthMethodRef.kba:
                    claim.ShouldBe(AuthMethodClaimValues.KBA);
                    break;
                case AuthMethodRef.face:
                    claim.ShouldBe(AuthMethodClaimValues.FACE);
                    break;
                case AuthMethodRef.fingerprint:
                    claim.ShouldBe(AuthMethodClaimValues.FINGERPRINT);
                    break;
                case AuthMethodRef.hwk:
                    claim.ShouldBe(AuthMethodClaimValues.HARDWARE_KEY);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected enum value: " + val);
            }
        }
    }

    //--------------------//

    [Fact]
    public void TryParseClaimValue_ShouldReturnTrue_AndCorrectEnum_ForKnownStrings()
    {
        static void AssertParse(string s, AuthMethodRef expected)
        {
            var result = AuthMethodClaimValueExtensions.TryParseClaimValue(s, out var parsed);
            result.ShouldBeTrue();
            parsed.ShouldBe(expected);
        }

        AssertParse(AuthMethodClaimValues.PASSWORD, AuthMethodRef.pwd);
        AssertParse(AuthMethodClaimValues.OTP, AuthMethodRef.otp);
        AssertParse(AuthMethodClaimValues.MULTI_FACTOR, AuthMethodRef.mfa);
        AssertParse(AuthMethodClaimValues.WINDOWS_INTEGRATED_AUTH, AuthMethodRef.windowsIntegratedAuth);
        AssertParse(AuthMethodClaimValues.RSA, AuthMethodRef.rsa);
        AssertParse(AuthMethodClaimValues.FEDERATED, AuthMethodRef.fed);
        AssertParse(AuthMethodClaimValues.FACE, AuthMethodRef.face);
        AssertParse(AuthMethodClaimValues.FINGERPRINT, AuthMethodRef.fingerprint);
        AssertParse(AuthMethodClaimValues.HARDWARE_KEY, AuthMethodRef.hwk);
    }

    //--------------------//

    [Fact]
    public void TryParseClaimValue_ShouldReturnFalse_ForUnknownOrNullStrings()
    {
        var unknownResult = AuthMethodClaimValueExtensions.TryParseClaimValue("unknown-value", out var unknownParsed);
        unknownResult.ShouldBeFalse();
        unknownParsed.ShouldBe(default);

        var nullResult = AuthMethodClaimValueExtensions.TryParseClaimValue(null, out var nullParsed);
        nullResult.ShouldBeFalse();
        nullParsed.ShouldBe(default);
    }
}

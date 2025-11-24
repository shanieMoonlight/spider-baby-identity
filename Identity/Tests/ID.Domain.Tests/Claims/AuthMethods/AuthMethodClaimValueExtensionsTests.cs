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
                case AuthMethodRef.Password:
                    claim.ShouldBe(AuthMethodClaimValues.PASSWORD);
                    break;
                case AuthMethodRef.Otp:
                    claim.ShouldBe(AuthMethodClaimValues.OTP);
                    break;
                case AuthMethodRef.Mfa:
                    claim.ShouldBe(AuthMethodClaimValues.MULTI_FACTOR);
                    break;
                case AuthMethodRef.WindowsIntegratedAuth:
                    claim.ShouldBe(AuthMethodClaimValues.WINDOWS_INTEGRATED_AUTH);
                    break;
                case AuthMethodRef.Rsa:
                    claim.ShouldBe(AuthMethodClaimValues.RSA);
                    break;
                case AuthMethodRef.Federated:
                    claim.ShouldBe(AuthMethodClaimValues.FEDERATED);
                    break;
                case AuthMethodRef.Face:
                    claim.ShouldBe(AuthMethodClaimValues.FACE);
                    break;
                case AuthMethodRef.Fingerprint:
                    claim.ShouldBe(AuthMethodClaimValues.FINGERPRINT);
                    break;
                case AuthMethodRef.HardwareKey:
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

        AssertParse(AuthMethodClaimValues.PASSWORD, AuthMethodRef.Password);
        AssertParse(AuthMethodClaimValues.OTP, AuthMethodRef.Otp);
        AssertParse(AuthMethodClaimValues.MULTI_FACTOR, AuthMethodRef.Mfa);
        AssertParse(AuthMethodClaimValues.WINDOWS_INTEGRATED_AUTH, AuthMethodRef.WindowsIntegratedAuth);
        AssertParse(AuthMethodClaimValues.RSA, AuthMethodRef.Rsa);
        AssertParse(AuthMethodClaimValues.FEDERATED, AuthMethodRef.Federated);
        AssertParse(AuthMethodClaimValues.FACE, AuthMethodRef.Face);
        AssertParse(AuthMethodClaimValues.FINGERPRINT, AuthMethodRef.Fingerprint);
        AssertParse(AuthMethodClaimValues.HARDWARE_KEY, AuthMethodRef.HardwareKey);
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

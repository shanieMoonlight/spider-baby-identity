using ID.Domain.Models;
using System.Text.Json.Serialization;

namespace ID.OAuth.Google.Features.SignIn.GoogleCookieSignIn;

public class CookieSignInResultData()
{
    public bool Succeeded { get; set; }

    public bool TwoFactorRequired { get; set; }

    /// <summary>
    /// How will 2 factor be verified
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TwoFactorProvider TwoFactorProvider { get; private set; } = TwoFactorProvider.Email; //Email always works

    public string Message { get; set; } = string.Empty;



    public static CookieSignInResultData CreateWithTwoFactoRequired(TwoFactorProvider provider, string message)
    {
        return new CookieSignInResultData()
        {
            Succeeded = false,
            TwoFactorRequired = true,
            TwoFactorProvider = provider,
            Message = message

        };
    }


    public static CookieSignInResultData Success(string? message = null)
    {
        return new CookieSignInResultData()
        {
            Succeeded = true,
            TwoFactorRequired = true,
            Message = message ?? "Signed In!"

        };
    }

}//Cls

using ID.Domain.Models;
using System.Text.Json.Serialization;

namespace ID.Application.Dtos.Account.Cookies;

public class CookieSignInResultData
{
    public bool Succeeded { get; set; }
    public bool EmailConfirmationRequired { get; set; }

    public bool TwoFactorRequired { get; set; }

    /// <summary>
    /// How will 2 factor be verified
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TwoFactorProvider TwoFactorProvider { get; set; } //Email always works

    public string Message { get; set; } = string.Empty;

    public string? ExtraInfo { get; set; } = string.Empty;



    public static CookieSignInResultData CreateWithTwoFactoRequired(TwoFactorProvider provider, string message) => new()
    {
        Succeeded = false,
        TwoFactorRequired = true,
        TwoFactorProvider = provider,
        Message = message

    };


    public static CookieSignInResultData Success(string? message = null) => new()
    {
        Succeeded = true,
        TwoFactorRequired = true,
        Message = message ?? "Signed In!"

    };


}//Cls

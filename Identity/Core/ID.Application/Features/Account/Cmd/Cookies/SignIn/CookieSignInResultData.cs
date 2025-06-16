using ID.Application.AppAbs.SignIn;
using ID.Domain.Models;
using System.Text.Json.Serialization;

namespace ID.Application.Features.Account.Cmd.Cookies.SignIn;
public class CookieSignInResultData(MyIdSignInResult signInResult)
{
    public bool Succeeded { get; set; } = signInResult.Succeeded;
    public bool EmailConfirmationRequired { get; set; } = signInResult.EmailConfirmationRequired;

    public bool TwoFactorRequired { get; set; } = signInResult.TwoFactorRequired;

    /// <summary>
    /// How will 2 factor be verified
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TwoFactorProvider TwoFactorProvider { get; private set; } = signInResult.MfaResultData?.TwoFactorProvider 
        ?? signInResult.User?.TwoFactorProvider 
        ?? TwoFactorProvider.Email; //Email always works

    public string Message { get; set; } = signInResult.Message;

    public string? ExtraInfo { get; set; } = signInResult.MfaResultData?.ExtraInfo;

}//Cls

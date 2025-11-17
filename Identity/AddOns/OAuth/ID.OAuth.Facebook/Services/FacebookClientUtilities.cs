using ID.OAuth.Facebook.Setup;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace ID.OAuth.Facebook.Services;
internal class FacebookClientUtilities(IOptions<IdOAuthFacebookOptions> optsProvider) : IFacebookClientUtilities
{

    private readonly IdOAuthFacebookOptions _opts = optsProvider.Value;

    //----------------------//


    public string GenerateAppSecretProof(string userAccessToken)
    {
        var appSecret = _opts.AppSecret ?? string.Empty;
        var key = Encoding.UTF8.GetBytes(appSecret);
        var tokenBytes = Encoding.UTF8.GetBytes(userAccessToken ?? string.Empty);

        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(tokenBytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

}//Cls

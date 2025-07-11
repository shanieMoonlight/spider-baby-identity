using Microsoft.IdentityModel.Tokens;

namespace ID.Infrastructure.Auth.JWT.Utils;
internal class KidIssuerSigningKeyResolver(IEnumerable<SecurityKey> _allKeys)
{
    public IEnumerable<SecurityKey> ResolveSigningKey(string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters)
    {
        if (string.IsNullOrEmpty(kid))
            return _allKeys; // fallback: try all keys

        return _allKeys.Where(k => k.KeyId == kid);
    }
}

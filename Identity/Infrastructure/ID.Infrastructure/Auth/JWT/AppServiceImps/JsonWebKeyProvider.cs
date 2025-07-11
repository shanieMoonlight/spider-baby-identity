using ID.Application.JWT;
using ID.Infrastructure.Auth.JWT.LocalServices.Abs;
using ID.Infrastructure.Auth.JWT.Setup;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace ID.Infrastructure.Auth.JWT.AppServiceImps;

public class JsonWebKeyProvider(
    IKeyProvider keyProvider,
    IOptions<JwtOptions> _jwtOptionsProvider) : IJsonWebKeyProvider
{
    private readonly JwtOptions _jwtOptions = _jwtOptionsProvider.Value;

    //--------------------------//        

    public Task<JwkListDto> GetJwks()
    {
        var validationKeys = keyProvider.GetAsymmetricValidationSigningKeys();
        var jwks = validationKeys
            .Select(ToJwksDto)
            .Where(jwk => jwk != null)
            .ToList();

        var dto = new JwkListDto(jwks);

        return Task.FromResult(dto);
    }


    //--------------------------//        


    private JwkDto ToJwksDto(RsaSecurityKey rsaKey)
    {
        RSAParameters parameters = rsaKey.Rsa != null
               ? rsaKey.Rsa.ExportParameters(false)
               : rsaKey.Parameters;

        string n = Base64UrlEncoder.Encode(parameters.Modulus);
        string e = Base64UrlEncoder.Encode(parameters.Exponent);

        return new JwkDto
        {
            Kty = "RSA",
            Use = "sig",
            Alg = _jwtOptions.AsymmetricAlgorithm,
            N = n,
            E = e,
            Kid = rsaKey.KeyId
        };
    }


}//Cls

using ID.Application.JWT;
using ID.Infrastructure.Auth.JWT.LocalServices.Abs;
using ID.Infrastructure.Auth.JWT.Setup;
using Microsoft.Extensions.Options;

namespace ID.Infrastructure.Auth.JWT.AppServiceImps;
internal class JwtKeyService(IKeyHelper _keyHelper, IOptions<JwtOptions> _jwtOptions)
    : IJwtKeyService
{
    private readonly JwtOptions _jwt = _jwtOptions.Value;


    public Task<string?> GetPublicSigningKeyAsync() =>
        _jwt.UseAsymmetricCrypto
        ? Task.FromResult<string?>(_keyHelper.ExportPublicKey())
        : Task.FromResult<string?>(null);

}

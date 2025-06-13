using ID.Application.JWT;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Refreshing;
using ID.Domain.Entities.Refreshing.ValueObjects;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.RefreshTokens;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace ID.Infrastructure.Auth.JWT.AppServiceImps;



/// <inheritdoc />
internal class JwtRefreshTokenService<TUser>(
    IIdUnitOfWork _uow, 
    IOptions<JwtOptions> _optionsProvider)
    : IJwtRefreshTokenService<TUser>
     where TUser : AppUser
{
    private readonly IIdentityRefreshTokenRepo _repo = _uow.RefreshTokenRepo;
    private readonly JwtOptions _options = _optionsProvider.Value;

    //-----------------------//  

    /// <inheritdoc />
    public async Task<IdRefreshToken?> FindTokenWithUserAndTeamAsync(string tknPayload, CancellationToken cancellationToken = default)
    {
        var spec = RefreshTokenWithUserAndTeamSpec.Create(tknPayload);
        return await _repo.FirstOrDefaultAsync(spec, cancellationToken);
    }

    //-----------------------//  

    /// <inheritdoc />
    public async Task RevokeTokensAsync(TUser user, CancellationToken cancellationToken = default)
    {
        var spec = RefreshTokenByUserIdSpec.Create(user);

        await _repo.RemoveRangeAsync(spec);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    //-----------------------//  

    /// <inheritdoc />
    public async Task<IdRefreshToken> GenerateTokenAsync(TUser user, CancellationToken cancellationToken)
    {
        var tokenPayload = JwtRefreshTokenService<TUser>.GeneratePayload();
        var token = IdRefreshToken.Create(
            TokenPayload.Create(tokenPayload),
            user,
            TokenLifetime.Create(_options.RefreshTokenTimeSpan));

        await _repo.AddAsync(token, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return token;
    }

    //-----------------------//   


    /// <inheritdoc />
    public async Task<IdRefreshToken> UpdateTokenPayloadAsync(IdRefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenPayload = JwtRefreshTokenService<TUser>.GeneratePayload();
        refreshToken.Update(
            TokenPayload.Create(tokenPayload),
            TokenLifetime.Create(_options.RefreshTokenTimeSpan)
            );

        await _repo.UpdateAsync(refreshToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return refreshToken;
    }

    //-----------------------//   

    private static string GeneratePayload()
    {
        var tokenLength = RandomNumberGenerator.GetInt32(100, 120);
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(tokenLength));
    }


}//Cls

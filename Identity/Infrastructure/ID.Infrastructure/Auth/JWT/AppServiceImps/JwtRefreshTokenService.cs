using ID.Application.JWT;
using ID.Domain.Claims.AuthMethods;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Refreshing;
using ID.Domain.Entities.Refreshing.ValueObjects;
using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Repos;
using ID.Domain.Repos.Specs.RefreshTokens;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ID.Infrastructure.Auth.JWT.AppServiceImps;



/// <inheritdoc />
internal class JwtRefreshTokenService<TUser>(
    IIdUnitOfWork _uow,
    IPasswordHasher<TUser> _passwordHasher,
    IOptions<JwtOptions> _optionsProvider)
    : IJwtRefreshTokenService<TUser>
     where TUser : AppUser
{
    private readonly IIdentityRefreshTokenRepo _repo = _uow.RefreshTokenRepo;
    private readonly JwtOptions _options = _optionsProvider.Value;

    //-----------------------//  

    /// <inheritdoc />
    public async Task<IdRefreshToken?> FindTokenWithUserAndDeviceAndTeamAsync(string tknPayload, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tknPayload))
            return null;

        // Expect client token in form selector.validator
        var parts = tknPayload.Split('.', 2);
        if (parts.Length != 2)
            return null;

        var selector = parts[0];
        var candidate = parts[1];

        //Use selector to find the token
        var spec = RefreshTokenBySelectorWithUserAndDeviceAndTeamSpec.Create(selector);
        var token = await _repo.FirstOrDefaultAsync(spec, cancellationToken);
        if (token == null)
            return null;

        //Token must have user, or something is wrong
        var user = (TUser?)token.User;
        if (user == null)
            return null;

        //Verify the candidate against the stored hash
        //var hasher = new PasswordHasher<TUser>();
        var verify = _passwordHasher.VerifyHashedPassword(user, token.PayloadHash, candidate);
        if (verify == PasswordVerificationResult.Failed)
            return null;

        if (verify == PasswordVerificationResult.SuccessRehashNeeded)
        {
            var newHash = _passwordHasher.HashPassword(user, candidate);
            token.PayloadHash = newHash;
            await _repo.UpdateAsync(token);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        return token;
    }

    //-----------------------//  

    /// <inheritdoc />
    public async Task<GeneratedTokenDto> GenerateAndStoreTokenAsync(
        TUser user,
        IEnumerable<AuthMethodRef> authMethodRefs,
        CancellationToken cancellationToken)
    {
        var validator = RandomTokenGenerator.Generate();
        var selector = RandomTokenGenerator.GenerateHashingSelector();

        //var hasher = new PasswordHasher<TUser>();
        var validatorHash = _passwordHasher.HashPassword(user, validator);

        var token = IdRefreshToken.Create(
            TokenPayloadHash.Create(validatorHash),
            TokenSelector.Create(selector),
            user,
            TokenLifetime.Create(_options.RefreshTokenTimeSpan),
            authMethodRefs);

        await _repo.AddAsync(token, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var clientToken = selector + "." + validator;
        return new GeneratedTokenDto(token, clientToken);
    }

    //-----------------------// 

    /// <inheritdoc />
    public async Task<GeneratedTokenDto> GenerateAndStoreWithDeviceAsync(
        TUser user,
        IEnumerable<AuthMethodRef> authMethodRefs,
        TrustedDevice trustedDevice,
        CancellationToken cancellationToken)
    {
        var validator = RandomTokenGenerator.Generate();
        var selector = RandomTokenGenerator.GenerateHashingSelector();

        var hasher = new PasswordHasher<TUser>();
        var validatorHash = hasher.HashPassword(user, validator);

        var token = IdRefreshToken.Create(
            TokenPayloadHash.Create(validatorHash),
            TokenSelector.Create(selector),
            user,
            TokenLifetime.Create(_options.RefreshTokenTimeSpan),
            authMethodRefs,
            trustedDevice);

        await _repo.AddAsync(token, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var clientToken = selector + "." + validator;
        return new GeneratedTokenDto(token, clientToken);
    }

    //-----------------------//    

    /// <inheritdoc />
    public async Task<IdRefreshToken> UpdateTokenPayloadAsync(IdRefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        var validator = RandomTokenGenerator.Generate();

        //var hasher = new PasswordHasher<TUser>();
        var validatorHash = _passwordHasher.HashPassword((TUser?)refreshToken.User!, validator);

        refreshToken.Update(
            TokenPayloadHash.Create(validatorHash),
            TokenLifetime.Create(_options.RefreshTokenTimeSpan)
            );

        await _repo.UpdateAsync(refreshToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return refreshToken;
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

    // GenerateSelector moved to RandomTokenGenerator.GenerateHashingSelector()

}//Cls

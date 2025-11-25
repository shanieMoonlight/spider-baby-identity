using ClArch.SimpleSpecification;
using ID.Domain.Claims.AuthMethods;
using ID.Domain.Repos.Specs.RefreshTokens;


namespace ID.Infrastructure.Tests.Auth.JWT.AppServiceImps;

public class JwtRefreshTokenServiceTests
{
    private readonly Mock<IIdUnitOfWork> _uow = new();
    private readonly Mock<IIdentityRefreshTokenRepo> _repo = new();

    private JwtRefreshTokenService<AppUser> CreateService(TimeSpan refresh)
    {
        var options = Options.Create(new JwtOptions { RefreshTokenTimeSpan = refresh });
        _uow.Setup(u => u.RefreshTokenRepo).Returns(_repo.Object);
        return new JwtRefreshTokenService<AppUser>(_uow.Object, options);
    }

    //-------------------------//

    [Fact]
    public async Task FindTokenWithUserAndDeviceAndTeamAsync_UsesCorrectSpec()
    {
        var payload = "sample-payload";
        var svc = CreateService(TimeSpan.FromDays(7));
        var user = AppUserDataFactory.Create();
        var device = TrustedDeviceDataFactory.Create(user: user);
        var token = RefreshTokenDataFactory.Create(user: user, trustedDevice: device, payload: payload);
        List<AuthMethodRef> authMethodRefs = [AuthMethodRef.face];

        _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ASimpleSpecification<IdRefreshToken>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var foundToken = await svc.FindTokenWithUserAndDeviceAndTeamAsync(payload, default);

        foundToken.ShouldNotBeNull();
        _repo.Verify(x => x.FirstOrDefaultAsync(It.IsAny<RefreshTokenByPayloadWithUserAndDeviceAndTeamSpec>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    //-------------------------//

    [Fact]
    public async Task GenerateTokenAsync_Creates_And_Saves_Token()
    {
        var svc = CreateService(TimeSpan.FromDays(7));
        var user = AppUserDataFactory.Create();
        List<AuthMethodRef> authMethodRefs = [AuthMethodRef.face];

        IdRefreshToken captured = null!;
        _repo.Setup(r => r.AddAsync(It.IsAny<IdRefreshToken>(), It.IsAny<CancellationToken>()))
            .Callback<IdRefreshToken, CancellationToken>((t, ct) => captured = t)
            .ReturnsAsync((IdRefreshToken t, CancellationToken ct) => t);

        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        var token = await svc.GenerateTokenAsync(user, authMethodRefs, default);

        token.ShouldNotBeNull();
        captured.ShouldNotBeNull();
        captured.UserId.ShouldBe(user.Id);
        captured.ExpiresOnUtc.ShouldBeGreaterThan(DateTime.UtcNow);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    //-------------------------//

    [Fact]
    public async Task GenerateTokenAsync_WithTrustedDevice_Associates_Device()
    {
        var svc = CreateService(TimeSpan.FromDays(7));
        var user = AppUserDataFactory.Create();
        var device = TrustedDeviceDataFactory.Create(user: user);
        List<AuthMethodRef> authMethodRefs = [AuthMethodRef.face, AuthMethodRef.fingerprint];

        IdRefreshToken captured = null!;
        _repo.Setup(r => r.AddAsync(It.IsAny<IdRefreshToken>(), It.IsAny<CancellationToken>()))
            .Callback<IdRefreshToken, CancellationToken>((t, ct) => captured = t)
            .ReturnsAsync((IdRefreshToken t, CancellationToken ct) => t);

        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        var token = await svc.GenerateTokenWithDeviceAsync(user, authMethodRefs, device, default);

        token.ShouldNotBeNull();
        captured.ShouldNotBeNull();
        captured.TrustedDevice.ShouldBe(device);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    //-----------------------//  

    [Fact]
    public async Task UpdateTokenPayloadAsync_Updates_And_Saves()
    {
        var svc = CreateService(TimeSpan.FromDays(7));
        var token = RefreshTokenDataFactory.Create();

        _repo.Setup(r => r.UpdateAsync(It.IsAny<IdRefreshToken>()))
            .ReturnsAsync(token);

        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        var updated = await svc.UpdateTokenPayloadAsync(token);

        updated.ShouldNotBeNull();
        updated.Payload.ShouldNotBeNullOrEmpty();
        updated.ExpiresOnUtc.ShouldBeGreaterThan(DateTime.UtcNow);
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}//Cls

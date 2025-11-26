using ClArch.SimpleSpecification;
using ID.Domain.Claims.AuthMethods;
using ID.Domain.Repos.Specs.RefreshTokens;
using Microsoft.AspNetCore.Identity;


namespace ID.Infrastructure.Tests.Auth.JWT.AppServiceImps;

public class JwtRefreshTokenServiceTests
{
    private readonly Mock<IIdUnitOfWork> _uow = new();
    private readonly Mock<IIdentityRefreshTokenRepo> _repo = new();
    private readonly Mock<IPasswordHasher<AppUser>> _pwdHasher = new();

    private JwtRefreshTokenService<AppUser> CreateService(TimeSpan refresh)
    {
        var options = Options.Create(new JwtOptions { RefreshTokenTimeSpan = refresh });
        _uow.Setup(u => u.RefreshTokenRepo).Returns(_repo.Object);

        // default hasher behaviour: verify succeeds
        _pwdHasher.Setup(h => h.VerifyHashedPassword(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);


        _pwdHasher.Setup(h => h.HashPassword(It.IsAny<AppUser>(), It.IsAny<string>())).Returns("hashed");

        return new JwtRefreshTokenService<AppUser>(_uow.Object, _pwdHasher.Object, options);
    }

    //-------------------------//

    [Fact]
    public async Task FindTokenWithUserAndDeviceAndTeamAsync_UsesCorrectSpec()
    {
        var payload = "sample-selector.sample-payload";
        var svc = CreateService(TimeSpan.FromDays(7));
        var user = AppUserDataFactory.Create();
        var device = TrustedDeviceDataFactory.Create(user: user);
        var token = RefreshTokenDataFactory.Create(user: user, trustedDevice: device, payload: payload);
        List<AuthMethodRef> authMethodRefs = [AuthMethodRef.face];

        _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ASimpleSpecification<IdRefreshToken>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var foundToken = await svc.FindTokenWithUserAndDeviceAndTeamAsync(payload, default);

        foundToken.ShouldNotBeNull();
        _repo.Verify(x => x.FirstOrDefaultAsync(It.IsAny<RefreshTokenBySelectorWithUserAndDeviceAndTeamSpec>(), It.IsAny<CancellationToken>()), Times.Once);
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

        var dto = await svc.GenerateAndStoreTokenAsync(user, authMethodRefs, default);

        dto.ShouldNotBeNull();
        dto.ClientToken.ShouldNotBeNullOrEmpty();
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

        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var dto = await svc.GenerateAndStoreWithDeviceAsync(user, authMethodRefs, device, default);

        dto.ShouldNotBeNull();
        dto.ClientToken.ShouldNotBeNullOrEmpty();
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

        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var updatedDto = await svc.UpdateTokenPayloadAsync(token);

        updatedDto.ShouldNotBeNull();
        updatedDto.RefreshToken.ShouldNotBeNull();
        // Payload was renamed to PayloadHash
        updatedDto.RefreshToken.PayloadHash.ShouldNotBeNullOrEmpty();
        updatedDto.RefreshToken.ExpiresOnUtc.ShouldBeGreaterThan(DateTime.UtcNow);
        updatedDto.ClientToken.ShouldNotBeNullOrEmpty();
        _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}//Cls

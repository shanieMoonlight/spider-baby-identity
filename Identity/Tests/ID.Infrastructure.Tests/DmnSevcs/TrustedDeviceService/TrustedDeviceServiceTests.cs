using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using ID.Infrastructure.DomainServices.Members;

namespace ID.Infrastructure.Tests.DmnSevcs.TrustedDeviceService;

public class TrustedDeviceServiceTests
{
    private readonly Mock<IIdUnitOfWork> _uowMock;

    public TrustedDeviceServiceTests()
    {
        _uowMock = new Mock<IIdUnitOfWork>();
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
    }

    //--------------------------//

    [Fact]
    public async Task AddAsync_Should_Add_Device_When_Validation_Succeeds()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var service = new TrustedDeviceService<AppUser>(_uowMock.Object);

        var fingerprint = DeviceFingerprint.Create("fp-1");
        var name = DeviceName.Create("name-1");
        var ua = UserAgent.CreateNullable("ua-1");

        // Act
        var result = await service.AddAsync(user, fingerprint, name, ua, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.DeviceFingerprint.ShouldBe("fp-1");
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    //--------------------------//

    [Fact]
    public async Task AddAsync_Should_Return_Failure_When_Max_Exceeded()
    {
        // Arrange
        // Fill user's trusted devices to max using factory pattern
        var userId = Guid.NewGuid();
        var max = ID.GlobalSettings.Setup.Defaults.IdGlobalDefaultValues.MAX_TRUSTED_DEVICES_PER_USER;
        var set = new HashSet<TrustedDevice>();
        for (int i = 0; i < max; i++)
            set.Add(TrustedDeviceDataFactory.Create(userId: userId));

        var user = AppUserDataFactory.Create(id: userId, trustedDevices: set);
        var service = new TrustedDeviceService<AppUser>(_uowMock.Object);

        var fingerprint = DeviceFingerprint.Create("fp-x");
        var name = DeviceName.Create("name-x");
        var ua = UserAgent.CreateNullable("ua-x");

        // Act
        var result = await service.AddAsync(user, fingerprint, name, ua, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
    }

    //--------------------------//

    [Fact]
    public async Task RevokeById_Should_Return_NotFound_When_Device_Not_Found()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var service = new TrustedDeviceService<AppUser>(_uowMock.Object);

        // Act
        var result = await service.RevokeAsync(user, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //--------------------------//

    [Fact]
    public async Task RevokeById_Should_Revoke_When_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var device = TrustedDeviceDataFactory.Create(userId: userId);
        var devices = new HashSet<TrustedDevice> { device };
        var user = AppUserDataFactory.Create(id: userId, trustedDevices: devices);

        var service = new TrustedDeviceService<AppUser>(_uowMock.Object);

        // Act
        var result = await service.RevokeAsync(user, device.Id, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Info.ToLowerInvariant().ShouldContain("revoked");
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    //--------------------------//

    [Fact]
    public async Task RevokeByFingerprint_Should_Return_NotFound_When_Device_Not_Found()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var service = new TrustedDeviceService<AppUser>(_uowMock.Object);

        // Act
        var result = await service.RevokeAsync(user, "nope", CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //--------------------------//

    [Fact]
    public async Task RevokeByFingerprint_Should_Revoke_When_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var device = TrustedDeviceDataFactory.Create(userId: userId, deviceFingerprint: "fp-1");
        var devices = new HashSet<TrustedDevice> { device };
        var user = AppUserDataFactory.Create(id: userId, trustedDevices: devices);

        var service = new TrustedDeviceService<AppUser>(_uowMock.Object);

        // Act
        var result = await service.RevokeAsync(user, "fp-1", CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Info.ToLowerInvariant().ShouldContain("revoked");
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

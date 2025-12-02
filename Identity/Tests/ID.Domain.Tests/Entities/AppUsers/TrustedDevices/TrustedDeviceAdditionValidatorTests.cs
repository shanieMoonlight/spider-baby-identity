using static MyResults.BasicResult;

namespace ID.Domain.Tests.Entities.AppUsers.TrustedDevices;

public class TrustedDeviceAdditionValidatorTests
{

    [Fact]
    public void Validate_WithValidUserAndDevice_ShouldReturnSuccessWithToken()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var fingerprint = DeviceFingerprint.Create($"fp-{Guid.NewGuid()}");
        var name = DeviceName.Create("My Device");
        var ua = UserAgent.Create("MyUserAgent");
        var ip = IpAddress.Create("IP");
        var trustDuration = TrustDuration.Create(TimeSpan.FromDays(7));

        // Act
        var result = TrustedDeviceValidators.Addition.Validate(user, fingerprint, name, ua,ip, trustDuration);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.User.ShouldBe(user);
        result.Value.DeviceFingerprint.ShouldBe(fingerprint);
        result.Value.DeviceName.ShouldBe(name);
        result.Value.UserAgent.ShouldBe(ua);
        result.Value.TrustDuration.ShouldBe(trustDuration);
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenUserAtMaxTrustedDevices_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var devices = Enumerable.Range(0, IdGlobalDefaultValues.MAX_TRUSTED_DEVICES_PER_USER)
            .Select(_ => TrustedDeviceDataFactory.Create(userId: userId))
            .ToHashSet();

        var user = AppUserDataFactory.Create(id: userId, trustedDevices: devices);

        var fingerprint = DeviceFingerprint.Create($"fp-{Guid.NewGuid()}");
        var name = DeviceName.Create("New Device");
        var ua = UserAgent.Create("UA");
        var ip = IpAddress.Create("IP");
        var trustDuration = TrustDuration.Create(TimeSpan.FromDays(1));

        // Act
        var result = TrustedDeviceValidators.Addition.Validate(user, fingerprint, name, ua,ip, trustDuration);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBeNull();
        result.Info.ShouldContain("maximum");
        result.Value.ShouldBeNull();
        result.Status.ShouldBe(ResultStatus.BadRequest);
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenDeviceAlreadyTrustedAndActive_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fpValue = $"fingerprint-{Guid.NewGuid()}";

        var existing = TrustedDeviceDataFactory.Create(userId: userId, deviceFingerprint: fpValue, trustedUntil: DateTime.UtcNow.AddDays(1));
        var devices = new HashSet<TrustedDevice> { existing };
        var user = AppUserDataFactory.Create(id: userId, trustedDevices: devices);

        var fingerprint = DeviceFingerprint.Create(fpValue);
        var name = DeviceName.Create("Some Device");
        var ua = UserAgent.Create("UA-String");
        var ip = IpAddress.Create("IP");    
        var trustDuration = TrustDuration.Create(TimeSpan.FromDays(3));

        // Act
        var result = TrustedDeviceValidators.Addition.Validate(user, fingerprint, name, ua,ip, trustDuration);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBeNull();
        result.Info.ShouldContain("trusted");
        result.Value.ShouldBeNull();
        result.Status.ShouldBe(ResultStatus.BadRequest);
    }

    //------------------------------------//

    [Fact]
    public void Validate_TokenShouldHaveInternalConstructor()
    {
        // Arrange & Act
        var tokenType = typeof(TrustedDeviceValidators.Addition.Token);
        var constructors = tokenType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        // Assert
        constructors.ShouldHaveSingleItem();
        var constructor = constructors.First();
        constructor.IsAssembly.ShouldBeTrue(); // internal
        constructor.IsPublic.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Validate_TokenShouldImplementIUserValidationToken()
    {
        // Arrange & Act
        var tokenType = typeof(TrustedDeviceValidators.Addition.Token);

        // Assert
        tokenType.GetInterfaces().ShouldContain(typeof(IUserValidationToken));
    }

    //------------------------------------//

    [Fact]
    public void Validate_SuccessfulValidation_TokenShouldContainCorrectData()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var fingerprint = DeviceFingerprint.Create($"fp-{Guid.NewGuid()}");
        var name = DeviceName.Create("Device Name");
        var ua = UserAgent.Create("UA-Example");
        var ip = IpAddress.Create("IP");
        var trustDuration = TrustDuration.Create(TimeSpan.FromHours(12));

        // Act
        var result = TrustedDeviceValidators.Addition.Validate(user, fingerprint, name, ua,ip, trustDuration);

        // Assert
        result.Succeeded.ShouldBeTrue();
        var token = result.Value!;

        token.User.ShouldBe(user);
        token.DeviceFingerprint.ShouldBe(fingerprint);
        token.DeviceName.ShouldBe(name);
        token.UserAgent.ShouldBe(ua);
        token.TrustDuration.ShouldBe(trustDuration);
    }

    //------------------------------------//

    [Fact]
    public void Validate_MultipleValidationErrors_ShouldReturnFirstError()
    {
        // Arrange - create user at max devices and also include a matching fingerprint
        var userId = Guid.NewGuid();
        var fpValue = $"samefp-{Guid.NewGuid()}";

        // create one device that matches fingerprint and is active
        var existing = TrustedDeviceDataFactory.Create(userId: userId, deviceFingerprint: fpValue, trustedUntil: DateTime.UtcNow.AddDays(1));

        var otherDevices = Enumerable.Range(0, IdGlobalDefaultValues.MAX_TRUSTED_DEVICES_PER_USER - 1)
            .Select(_ => TrustedDeviceDataFactory.Create(userId: userId))
            .ToList();

        var devices = new HashSet<TrustedDevice>(otherDevices) { existing };
        var user = AppUserDataFactory.Create(id: userId, trustedDevices: devices);

        var fingerprint = DeviceFingerprint.Create(fpValue);
        var name = DeviceName.Create("Device X");
        var ua = UserAgent.Create("UA");
        var ip = IpAddress.Create("IP");
        var trustDuration = TrustDuration.Create(TimeSpan.FromDays(2));

        // Act
        var result = TrustedDeviceValidators.Addition.Validate(user, fingerprint, name, ua, ip, trustDuration);

        // Assert - should return the first error (max devices)
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBeNullOrEmpty();
        result.Info.ShouldContain("maximum");
    }

}

using ID.Domain.Entities.AppUsers.Validators;
using ID.Domain.Entities.TrustedDevices;
using ID.Tests.Data.Factories;
using Shouldly;
using System;
using System.Linq;
using System.Reflection;
using Xunit;
using static MyResults.BasicResult;

namespace ID.Domain.Tests.Entities.AppUsers.TrustedDevices;

public class TrustedDeviceRevocationValidatorTests
{

    [Fact]
    public void Validate_WithOwnerAndDevice_ShouldReturnSuccessWithToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var device = TrustedDeviceDataFactory.Create(userId: userId);
        var user = AppUserDataFactory.Create(id: userId, trustedDevices: new HashSet<TrustedDevice> { device });

        // Act
        var result = TrustedDeviceValidators.Revocation.Validate(user, device);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.User.ShouldBe(user);
        result.Value.Device.ShouldBe(device);
    }

    //-----------------------//

    [Fact]
    public void Validate_WhenDeviceNotOwnedByUser_ShouldReturnFailure()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var device = TrustedDeviceDataFactory.Create(userId: ownerId);

        var otherUser = AppUserDataFactory.Create(); // different id

        // Act
        var result = TrustedDeviceValidators.Revocation.Validate(otherUser, device);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBeNull();
        result.Value.ShouldBeNull();
        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }

    //-----------------------//

    [Fact]
    public void Validate_TokenShouldHaveInternalConstructor()
    {
        // Arrange & Act
        var tokenType = typeof(TrustedDeviceValidators.Revocation.Token);
        var constructors = tokenType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        // Assert
        constructors.ShouldHaveSingleItem();
        var constructor = constructors.First();
        constructor.IsAssembly.ShouldBeTrue(); // internal
        constructor.IsPublic.ShouldBeFalse();
    }

    //-----------------------//

    [Fact]
    public void Validate_TokenShouldImplementIUserValidationToken()
    {
        // Arrange & Act
        var tokenType = typeof(TrustedDeviceValidators.Revocation.Token);

        // Assert
        tokenType.GetInterfaces().ShouldContain(typeof(IUserValidationToken));
    }

    //-----------------------//

    [Fact]
    public void Validate_SuccessfulValidation_TokenShouldContainCorrectData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var device = TrustedDeviceDataFactory.Create(userId: userId);
        var user = AppUserDataFactory.Create(id: userId, trustedDevices: new HashSet<TrustedDevice> { device });

        // Act
        var result = TrustedDeviceValidators.Revocation.Validate(user, device);

        // Assert
        result.Succeeded.ShouldBeTrue();
        var token = result.Value!;

        token.User.ShouldBe(user);
        token.Device.ShouldBe(device);
    }

}

using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.Tests.Utility;
using ID.Infrastructure.TokenServices;
using ID.Infrastructure.TokenServices.Email;
using ID.Infrastructure.TokenServices.Phone;
using ID.Infrastructure.TokenServices.Pwd;
using ID.Infrastructure.TokenServices.TwoFactor;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Tests.TokenServices;

public class TokenServicesSetupTests
{

    //------------------------------------//

    [Theory]
    [MemberData(nameof(GetServiceRegistrations), parameters: true)]
    public void AddTokenBasedServices_UseDbTokenProvider_True_RegistersDbServices(Type serviceType, Type implementationType)
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.UseDbTokenProvider = true;

        // Act
        services.AddTokenBasedServices<AppUser>(true);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = services.FirstOrDefault(s => s.ServiceType == serviceType);
        service?.ImplementationType.ShouldBe(implementationType);
    }

    //------------------------------------//

    [Theory]
    [MemberData(nameof(GetServiceRegistrations), parameters: false)]
    public void AddTokenBasedServices_UseDbTokenProvider_False_RegistersDefaultServices(Type serviceType, Type implementationType)
    {
        // Arrange
        var services = new ServiceCollection();
        var setupOptions = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        setupOptions.UseDbTokenProvider = false;

        // Act
        services.AddTokenBasedServices<AppUser>(false);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = services.FirstOrDefault(s => s.ServiceType == serviceType);
        service?.ImplementationType.ShouldBe(implementationType);
    }

    //------------------------------------//

    public static TheoryData<Type, Type> GetServiceRegistrations(bool useDbTokenProvider)
    {
        var data = new TheoryData<Type, Type>();

        if (useDbTokenProvider)
        {
            data.Add(typeof(IEmailConfirmationService<AppUser>), typeof(DbEmailConfirmationService<AppUser>));
            data.Add(typeof(IPwdResetService<AppUser>), typeof(DbPwdResetService<AppUser>));
            data.Add(typeof(ITwoFactorVerificationService<AppUser>), typeof(DbTwoFactorVerificationService<AppUser>));
            data.Add(typeof(IPhoneConfirmationService<AppUser>), typeof(DbPhoneConfirmationService<AppUser>));
        }
        else
        {
            data.Add(typeof(IEmailConfirmationService<AppUser>), typeof(DefaultEmailConfirmationService<AppUser>));
            data.Add(typeof(IPwdResetService<AppUser>), typeof(DefaultPwdResetService<AppUser>));
            data.Add(typeof(ITwoFactorVerificationService<AppUser>), typeof(DefaultTwoFactorVerificationService<AppUser>));
            data.Add(typeof(IPhoneConfirmationService<AppUser>), typeof(DefaultPhoneConfirmationService<AppUser>));
        }

        return data;
    }

    //------------------------------------//

}//Cls

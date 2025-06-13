using ID.Application.AppAbs.ApplicationServices;
using ID.Application.AppAbs.ApplicationServices.Principal;
using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.RequestInfo;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppImps.Permissions;
using ID.Application.AppImps.RequestInfo;
using ID.Application.AppImps.SignIn;
using ID.Application.AppImps.TwoFactor;
using ID.Application.AppImps.User;
using ID.Domain.Entities.AppUsers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Application.AppImps;

public static class ApplicationImplementationsSetupExtensions
{

    /// <summary>
    /// Setup internal Services used by Features in IdApplication
    /// </summary>
    /// <param name="services"></param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddApplicationImplementations<TUser>(this IServiceCollection services) where TUser : AppUser
    {
        services.AddAppPermissions<TUser>();

        services.TryAddScoped<IFindUserService<TUser>, FindUserService<TUser>>();
        services.TryAddScoped<IIdPrincipalInfo, IdPrincipalInfo>();
        
        services.TryAddScoped<IUserInfo, UserInfo>();

        services.TryAddScoped<IEmailConfirmationBus, EmailConfirmationBus>();

        services.TryAddScoped<ITwoFactorMsgService, TwoFactorMsgService>();
        services.TryAddScoped<ITwoFactorCompleteRegistrationHandler, TwoFactorCompleteRegistrationHandler>();
        services.TryAddScoped<IPreSignInService<AppUser>, PreSignInService<AppUser>>();


        return services;
    }

}//Cls


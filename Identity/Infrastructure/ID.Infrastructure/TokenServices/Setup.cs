using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.JWT;
using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.Auth.JWT.AppServiceImps;
using ID.Infrastructure.TokenServices.Email;
using ID.Infrastructure.TokenServices.Phone;
using ID.Infrastructure.TokenServices.Pwd;
using ID.Infrastructure.TokenServices.TwoFactor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Infrastructure.TokenServices;


internal static class TokenServicesSetup
{
    internal static IServiceCollection AddTokenBasedServices<TUser>(this IServiceCollection services, bool useDbTokenProvider) where TUser : AppUser
    {
            services.TryAddTransient<IJwtRefreshTokenService<AppUser>, JwtRefreshTokenService<AppUser>>();

        if (useDbTokenProvider)
        {
            services.TryAddTransient<IEmailConfirmationService<TUser>, DbEmailConfirmationService<TUser>>();
            services.TryAddTransient<IPwdResetService<TUser>, DbPwdResetService<TUser>>();
            services.TryAddTransient<ITwoFactorVerificationService<TUser>, DbTwoFactorVerificationService<TUser>>();
            services.TryAddTransient<IPhoneConfirmationService<TUser>, DbPhoneConfirmationService<TUser>>();
        }
        else
        {
            services.TryAddTransient<IEmailConfirmationService<TUser>, DefaultEmailConfirmationService<TUser>>();
            services.TryAddTransient<IPwdResetService<TUser>, DefaultPwdResetService<TUser>>();
            services.TryAddTransient<ITwoFactorVerificationService<TUser>, DefaultTwoFactorVerificationService<TUser>>();
            services.TryAddTransient<IPhoneConfirmationService<TUser>, DefaultPhoneConfirmationService<TUser>>();
        }

        return services;
    }

}

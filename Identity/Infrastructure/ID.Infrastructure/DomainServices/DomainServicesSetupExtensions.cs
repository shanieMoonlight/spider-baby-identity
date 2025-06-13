using ID.Domain.Abstractions.Services.Members;
using ID.Domain.Abstractions.Services.Outbox;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Abstractions.Services.Teams.Dvcs;
using ID.Domain.Abstractions.Services.Teams.Subs;
using ID.Domain.Abstractions.Services.Transactions;
using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.DomainServices.Members;
using ID.Infrastructure.DomainServices.Outbox;
using ID.Infrastructure.DomainServices.SubPlans;
using ID.Infrastructure.DomainServices.Teams;
using ID.Infrastructure.DomainServices.Teams.Dvcs;
using ID.Infrastructure.DomainServices.Teams.Subs;
using ID.Infrastructure.DomainServices.Transactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Infrastructure.DomainServices;

public static class DomainServicesSetupExtensions
{
    /// <summary>
    /// Configures all  DomainServices teamMgr, UserMgr, etc
    /// </summary>
    /// <param name="services">Collection of services</param>
    internal static IServiceCollection AddDomainServices<TUser>(this IServiceCollection services) where TUser : AppUser
    {
        services.TryAddScoped<IIdUserMgmtService<TUser>, MyIdUserMgmtService<TUser>>();
        services.TryAddScoped<IIdUserMgmtUtilityService<TUser>, MyIdUserMgmtUtilityService<TUser>>();
        services.TryAddScoped<IIdentityMemberAuditService<TUser>, IdMemberAuditService<TUser>>();
        services.TryAddScoped<IIdentityTeamManager<TUser>, TeamManagerService<TUser>>();


        services.TryAddScoped<ITeamSubscriptionServiceFactory, TeamSubsriptionServiceFactory>();
        services.TryAddScoped<ITeamDeviceServiceFactory, TeamDeviceServiceFactory>();
        //services.TryAddScoped<IIdCustomerAuditService, IdCustomerAuditService>();


        services.TryAddScoped<IIdentityFeatureFlagService, IdentityFeatureFlagService>();
        services.TryAddScoped<IIdentitySubscriptionPlanService, IdentitySubscriptionPlanService>();


        services.TryAddScoped<IIdentityOutboxMsgsService, IdentityOutboxMsgsService>();


        services.TryAddScoped<IIdentityTransactionService, IdentityTransactionService>();

        return services;


    }

}//Cls


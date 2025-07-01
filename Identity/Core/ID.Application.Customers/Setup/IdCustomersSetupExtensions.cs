using FluentValidation;
using ID.Application.Customers.Abstractions;
using ID.Application.Customers.ApplicationImps;
using ID.Application.Mediatr;
using ID.Application.Mediatr.Behaviours.Validation;
using ID.GlobalSettings.Setup;
using ID.GlobalSettings.Setup.Options;
using MediatR;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Application.Customers.Setup;

public static class IdCustomersSetupExtensions
{

    /// <summary>
    /// Setup IdDomain
    /// </summary>
    /// <param name="services"></param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdCustomers(this IServiceCollection services, IdGlobalSetupOptions_CUSTOMER setupOptions)
    {
        var assembly = typeof(IdApplicationCustomersAssemblyReference).Assembly;

        //services.AddMyIdMediatr(assembly);

        services.AddMediatR(config =>
        {     // Loads team context from principal
            config.RegisterServicesFromAssembly(assembly);
        });

        // Add FluentValidation validators from the Customers assembly
        services.AddValidatorsFromAssembly(assembly);

        services.AddControllers()
                .PartManager.ApplicationParts.Add(new AssemblyPart(assembly));

        services.TryAddScoped<IIdCustomerRegistrationService, IdCustomerRegistrationService>();

        services.Configure_CUSTOMER_GlobalSettings(setupOptions);

        return services;
    }

    //----------------------//

    /// <summary>
    /// Setup Customer auth (Customer Accounts url is required)
    /// </summary>
    /// <param name="services"></param>
    /// <returns>The same services</returns>
    public static IServiceCollection AddMyIdCustomers(this IServiceCollection services, Action<IdGlobalSetupOptions_CUSTOMER> config)
    {
        IdGlobalSetupOptions_CUSTOMER setupOptions = new();
        config?.Invoke(setupOptions);

        return services.AddMyIdCustomers(setupOptions);
    }

}//Cls

using ID.Application.Mediatr.Behaviours;
using ID.Application.Mediatr.Behaviours.Exceptions;
using ID.Application.Mediatr.Behaviours.Validation;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace ID.Application.Mediatr;
internal static class MediatrSetup
{
    /// <summary>    
    /// This method configures the MediatR pipeline with behaviors in a specific order that must be maintained:
    /// 1. Principal behavior - Establishes security context from request
    /// 2. UserAware behavior - Loads user entity from principal context  
    /// 3. TeamAware behavior - Establishes team context from user
    /// 4. Validation behavior - Validates requests with full context available
    /// 5. Transaction behavior - Wraps command execution in database transactions
    /// </summary>
    /// <param name="assembly">Assembly with mediatr features and events</param>
    internal static IServiceCollection AddMyIdMediatr(this IServiceCollection services, Assembly assembly)
    {
        services.TryAddTransient(typeof(IRequestExceptionHandler<,,>), typeof(IdRequestExceptionHandler<,,>));


        services.AddMediatR(config =>
        {
            // CRITICAL: Pipeline execution order creates dependency chain - DO NOT REORDER
            // 1. Principal behavior extracts user info from request context (required by downstream behaviors)
            // 2. UserAware behavior uses principal info to load user entity
            // 3. TeamAware behavior uses principal info to establish team context  
            // 4. Validation runs with full context (principal + user + team)
            // 5. Transaction wraps the final command execution

            config.AddOpenBehavior(typeof(IdPrincipalPipelineBehavior<,>));        // Establishes principal context
            config.AddOpenBehavior(typeof(IdUserAwarePipelineBehavior<,>));        // Loads user from principal
            config.AddOpenBehavior(typeof(IdTeamAwarePipelineBehavior<,>));        // Loads team context from principal
            config.AddOpenBehavior(typeof(IdValidationPipelineBehaviour<,>));      // Validates with full context
            config.AddOpenBehavior(typeof(IdCommandTransactionPipelineBehaviour<,>)); // Wraps in transaction

            config.RegisterServicesFromAssembly(assembly);
        });



        return services;

    }


}

using ID.Application.Authenticators.Abstractions;
using ID.Application.Authenticators.Utils;
using ID.Application.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Application.Authenticators.Teams;

/// <summary>
/// Provides authorization for super team members with development environment bypass capability.
/// Combines position-based authorization with development environment accessibility for improved productivity.
/// </summary>
/// <remarks>
/// <para><strong>Use Case:</strong> High-privilege operations that should be accessible in development but restricted in production.</para>
/// <para><strong>Authorization Logic:</strong> Development environment OR (Super team membership AND position level >= minimum).</para>
/// <para><strong>Common Scenarios:</strong> Admin panels, debugging endpoints, system diagnostics, configuration tools.</para>
/// <para><strong>Developer Benefit:</strong> Allows full access during development while maintaining production security.</para>
/// </remarks>
public static class SuperMinimumOrDevAuthenticator
{
    /// <summary>
    /// Core authorization handler with development environment bypass functionality.
    /// Provides flexible authorization that adapts to deployment environment.
    /// </summary>
    /// <remarks>
    /// <para>In development environments, all authenticated users are granted access.</para>
    /// <para>In production environments, requires super team membership with position hierarchy.</para>
    /// </remarks>
    protected internal class AuthHandler : IAuthPositionHandler
    {
        /// <summary>
        /// Validates authorization with development environment bypass or super team position requirements.
        /// </summary>
        /// <param name="context">HTTP context containing authenticated user claims and environment information</param>
        /// <param name="level">Minimum position level required for super team members in production environments</param>
        /// <returns>
        /// <c>true</c> if running in development environment OR (user is in super team AND position level meets requirement);
        /// <c>false</c> if in production without sufficient super team authorization
        /// </returns>
        public bool IsAuthorized(HttpContext context, int? level = -1)
        {
            level ??= -1;
            var isDev = context.IsDevEnv();
            var isInSprTeam = context.User.IsInSuperTeam();
            var teamPosition = context.User.TeamPosition();

            return isDev
                ||
                isInSprTeam && teamPosition >= level;
        }

    }//Cls


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//

    #region FiltersAndPolicies

    /// <summary>
    /// <inheritdoc cref="AuthHandler"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ResourceFilter() : Attribute, IAsyncResourceFilter
    {
        private readonly int? _level;

        public ResourceFilter(int level) : this() =>
            _level = level;

        public async Task OnResourceExecutionAsync(
            ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var httpContext = context.HttpContext;

            if (httpContext.IsDevEnv())
                await next();
            else if (!httpContext.IsAuthenticated())
                context.Result = new UnauthorizedResult();
            else if (new AuthHandler().IsAuthorized(context.HttpContext, _level))
                await next();
            else
                context.Result = new ForbidResult();
        }

    }//Cls


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//

    /// <summary>
    /// <inheritdoc cref="AuthHandler"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ActionFilter() : ActionFilterAttribute
    {
        private readonly int? _level;

        public ActionFilter(int level) : this() =>
            _level = level;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;

            if (httpContext.IsDevEnv())
                base.OnActionExecuting(context);
            else if (!httpContext.IsAuthenticated())
                context.Result = new UnauthorizedResult();
            else if (new AuthHandler().IsAuthorized(context.HttpContext, _level))
                base.OnActionExecuting(context);
            else
                context.Result = new ForbidResult();
        }

    }//Cls


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//

    public static class Policy
    {
        public const string NAME = $"{nameof(SuperAuthenticator)}.{nameof(Policy)}";

        //------------------------------//

        /// <summary>
        /// Set to null to allow any position
        /// </summary>
        /// <param name="Level"></param>
        public record Requirement(int? Level) : IAuthorizationRequirement;

        //------------------------------//

        public class Handler(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<Requirement>
        {
            private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, Requirement requirement)
            {
                AuthHandler baseHandler = new();
                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext == null)
                    context.Fail();
                else if (httpContext.IsDevEnv())
                    context.Fail();
                else if (!httpContext.IsAuthenticated())
                    context.Fail();
                else if (!baseHandler.IsAuthorized(httpContext, requirement.Level))
                    context.Fail();
                else
                    context.Succeed(requirement);

                return Task.CompletedTask;
            }
        }

        //------------------------------//

        public class Attribute : AuthorizeAttribute
        {
            /// <summary>
            /// <inheritdoc cref="AuthHandler"/>
            /// </summary>
            public Attribute() => Policy = $"{NAME}";
        }

    }//Cls 
    #endregion


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//

}//Cls



//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
//#-#-#-#-#-#      SETUP     #-#-#-#-#-#//
//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//


public static class SuperMinimumOrDevAuthenticatorPolicySetupExtensions
{
    public static IServiceCollection AddSuperMinimumOrDevAuthenticatorPolicy(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, SuperMinimumOrDevAuthenticator.Policy.Handler>();
        return services.AddAuthorization(options =>
              options.AddPolicy(SuperMinimumOrDevAuthenticator.Policy.NAME, policy =>
              {
                  policy
                    //.RequireAuthenticatedUser() //This is handled in the Handler
                    .Requirements.Add(new SuperMinimumOrDevAuthenticator.Policy.Requirement(null));
              })
        );
    }
}//Cls


//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
//#-#-#-#-#-#      SETUP     #-#-#-#-#-#//
//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
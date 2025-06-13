using ID.Application.Authenticators.Abstractions;
using ID.Application.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Application.Authenticators.Teams;

/// <summary>
/// Provides authorization for super team members with minimum position level requirements.
/// This is the primary authenticator for high-privilege operations requiring specific seniority.
/// </summary>
/// <remarks>
/// <para><strong>Use Case:</strong> Production operations requiring senior-level super team members.</para>
/// <para><strong>Authorization Logic:</strong> Super team membership AND position level >= minimum required.</para>
/// <para><strong>Common Scenarios:</strong> System configuration, production deployments, sensitive admin operations.</para>
/// <para><strong>Example:</strong> [SuperMinimumAuthenticator.ResourceFilter(5)] requires super team members with position >= 5.</para>
/// </remarks>
public static class SuperMinimumAuthenticator
{
    protected internal class AuthHandler : IAuthPositionHandler  //protected internal for testing
    {
        /// <summary>
        /// Validates super team membership and minimum position level authorization.
        /// </summary>
        /// <param name="context">HTTP context containing authenticated user claims and team information</param>
        /// <param name="level">Minimum position level required. Use -1 for any super team member, higher numbers for senior positions</param>
        /// <returns>
        /// <c>true</c> if user is in super team AND position level meets or exceeds requirement;
        /// <c>false</c> if user lacks super team membership or insufficient position level
        /// </returns>
        public bool IsAuthorized(HttpContext context, int? level = -1)
        {
            level ??= -1;
            var isInSprTeam = context.User.IsInSuperTeam();
            var teamPosition = context.User.TeamPosition();
            return isInSprTeam && teamPosition >= level;
        }

    }//Cls


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//

    #region FiltersAndPolicies    

    /// <summary>
    /// Resource filter attribute for super team minimum position authorization.
    /// Implements early-stage authorization before resource allocation.
    /// </summary>
    /// <remarks>
    /// <para>Apply to controllers or actions requiring super team members with minimum position levels.</para>
    /// <para>Executes during resource execution phase for efficient auth checking.</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.All)]
    public class ResourceFilter() : Attribute, IAsyncResourceFilter
    {
        private readonly int? _level;

        public ResourceFilter(int level) : this() =>
            _level = level;

        public async Task OnResourceExecutionAsync(
            ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (!context.HttpContext.IsAuthenticated())
                context.Result = new UnauthorizedResult();
            else if (new AuthHandler().IsAuthorized(context.HttpContext, _level))
                await next();
            else
                context.Result = new ForbidResult();
        }

    }//Cls


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//   

    /// <summary>
    /// Action filter attribute for super team minimum position authorization.
    /// Provides fine-grained authorization control at the action level.
    /// </summary>
    /// <remarks>
    /// <para>Use when you need position-level authorization for specific actions within a controller.</para>
    /// <para>Executes during action execution phase, allowing for granular control.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// [SuperMinimumAuthenticator.ActionFilter(5)]
    /// public IActionResult SeniorOperation() { }
    /// 
    /// [SuperMinimumAuthenticator.ActionFilter()]  // Any super team member
    /// public IActionResult BasicOperation() { }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.All)]
    public class ActionFilter() : ActionFilterAttribute
    {
        private readonly int _level = -1;

        public ActionFilter(int level) : this() =>
            _level = level;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.IsAuthenticated())
                context.Result = new UnauthorizedResult();
            else if (new AuthHandler().IsAuthorized(context.HttpContext, _level))
                base.OnActionExecuting(context);
            else
                context.Result = new ForbidResult();
        }

    }//Cls


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//

    /// <summary>
    /// Authorization policy components for dependency injection and declarative authorization.
    /// Provides policy-based authorization for super team minimum position requirements.
    /// </summary>
    /// <remarks>
    /// <para>Use with [Authorize] attributes and policy-based authorization patterns.</para>
    /// <para>Register with AddSuperMinimumAuthenticatorPolicy() during service configuration.</para>
    /// </remarks>
    public static class Policy
    {
        public const string NAME = $"{nameof(SuperAuthenticator)}.{nameof(Policy)}";

        //------------------------------//        /// <summary>
        /// Authorization requirement specifying minimum position level for super team members.
        /// </summary>
        /// <param name="Level">
        /// Minimum position level required. Use null or -1 for any super team member,
        /// positive integers for specific position requirements
        /// </param>
        public record Requirement(int? Level) : IAuthorizationRequirement;

        //------------------------------//

        /// <summary>
        /// Authorization policy handler for dependency injection-based authorization.
        /// Integrates with ASP.NET Core's authorization pipeline.
        /// </summary>
        public class Handler(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<Requirement>
        {
            private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, Requirement requirement)
            {
                AuthHandler baseHandler = new();
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null || !baseHandler.IsAuthorized(httpContext, requirement.Level))
                    context.Fail();
                else
                    context.Succeed(requirement);

                return Task.CompletedTask;
            }
        }

        //------------------------------//      

        /// <summary>
        /// Authorize attribute for policy-based super team minimum position authorization.
        /// Use with ASP.NET Core's built-in [Authorize] pattern.
        /// </summary>
        /// <example>
        /// <code>
        /// [SuperMinimumAuthenticator.Policy.Attribute]
        /// public class SecureController : ControllerBase { }
        /// </code>
        /// </example>
        public class Attribute : AuthorizeAttribute
        {
            /// <summary>
            /// Initializes the authorize attribute with super team minimum position policy.
            /// Uses the default policy configuration set during service registration.
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


/// <summary>
/// Extension methods for configuring SuperMinimumAuthenticator in the dependency injection container.
/// </summary>
public static class SuperMinimumAuthenticatorPolicySetupExtensions
{
    /// <summary>
    /// Registers the SuperMinimumAuthenticator policy and authorization handlers.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <returns>The configured service collection for method chaining</returns>
    /// <remarks>
    /// <para>Call this during startup to enable policy-based super team minimum position authorization.</para>
    /// <para>The default policy allows any super team member (no minimum position).</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // In Program.cs or Startup.cs
    /// services.AddSuperMinimumAuthenticatorPolicy();
    /// </code>
    /// </example>
    public static IServiceCollection AddSuperMinimumAuthenticatorPolicy(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, SuperMinimumAuthenticator.Policy.Handler>();
        return services.AddAuthorization(options =>
              options.AddPolicy(SuperMinimumAuthenticator.Policy.NAME, policy =>
                  policy
                    .RequireAuthenticatedUser()
                    .Requirements.Add(new SuperMinimumAuthenticator.Policy.Requirement(null)))
        );
    }

}//Cls


//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
//#-#-#-#-#-#      SETUP     #-#-#-#-#-#//
//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
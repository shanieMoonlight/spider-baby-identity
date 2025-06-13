using ID.Application.Authenticators.Abstractions;
using ID.Application.Utility.ExtensionMethods;
using ID.TeamRoles.UserToAdmin.Authenticators.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ID.TeamRoles.UserToAdmin.Authenticators.Mntc;

/// <summary>
/// Only a Super Team or Mntc team at Mgr or higher shall pass!
/// </summary>
public static class MntcManagerMinimumAuthenticator
{
    /// <summary>
    /// <inheritdoc cref="MntcManagerMinimumAuthenticator"/>
    /// </summary>
    protected internal class AuthHandler : IAuthHandler
    {
        /// <summary>
        /// Determines whether the user is authorized based on their team and role.
        /// </summary>
        /// <param name="context">The HTTP context containing the user information.</param>
        /// <returns>True if the user is authorized; otherwise, false.</returns>
        public bool IsAuthorized(HttpContext context) =>
            context.IsHigherThanMntc()
            ||
            (context.IsInMntcTeam() && context.IsMgrMin());

    }//Cls


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//

    #region FiltersAndPolicies

    /// <summary>ActionFilter
    /// <inheritdoc cref="AuthHandler"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ResourceFilter : Attribute, IAsyncResourceFilter
    {
        #region CTOR for Docs
        /// <summary>
        /// <inheritdoc cref="AuthHandler"/>
        /// </summary>
        public ResourceFilter() { }
        #endregion

        public async Task OnResourceExecutionAsync(
            ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (!context.HttpContext.IsAuthenticated())
                context.Result = new UnauthorizedResult();
            else if (new AuthHandler().IsAuthorized(context.HttpContext))
                await next();
            else
                context.Result = new ForbidResult();
        }
    }//Cls


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//


    /// <summary>ActionFilter
    /// <inheritdoc cref="AuthHandler"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ActionFilter : ActionFilterAttribute
    {
        #region CTOR for Docs
        /// <summary>
        /// <inheritdoc cref="AuthHandler"/>
        /// </summary>
        public ActionFilter() { }
        #endregion

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.IsAuthenticated())
                context.Result = new UnauthorizedResult();
            else if (new AuthHandler().IsAuthorized(context.HttpContext))
                base.OnActionExecuting(context);
            else
                context.Result = new ForbidResult();
        }

    }//Cls


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//

    public static class Policy
    {
        public const string NAME = $"{nameof(MntcManagerMinimumAuthenticator)}.{nameof(Policy)}";

        //------------------------------//

        public class Requirement : IAuthorizationRequirement { }

        //------------------------------//

        public class Handler(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<Requirement>
        {
            private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, Requirement requirement)
            {
                AuthHandler baseHandler = new();
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null || !baseHandler.IsAuthorized(httpContext))
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


public static class MntcManagerMinimumAuthenticatorPolicySetupExtensions
{
    public static IServiceCollection AddMntcManagerMinimumAuthenticatorPolicy(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, MntcManagerMinimumAuthenticator.Policy.Handler>();
        return services.AddAuthorization(options =>
              options.AddPolicy(MntcManagerMinimumAuthenticator.Policy.NAME, policy =>
                  policy
                  .RequireAuthenticatedUser()
                  .Requirements.Add(new MntcManagerMinimumAuthenticator.Policy.Requirement()))
        );
    }
}//Cls


//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
//#-#-#-#-#-#      SETUP     #-#-#-#-#-#//
//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
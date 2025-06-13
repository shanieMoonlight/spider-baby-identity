using ID.Application.Authenticators.Abstractions;
using ID.Application.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Application.Authenticators.Teams;

public static class CustomerLeaderMinimumAuthenticator
{    /// <summary>
    /// Authorizes customer team leaders or members of higher teams.
    /// Use for customer management operations that require leadership authority
    /// but should be accessible to admin teams.
    /// </summary>
    protected internal class AuthHandler : IAuthHandler
    {
        /// <summary>
        /// Determines authorization for customer leadership operations.
        /// </summary>
        /// <param name="context">HTTP context containing user claims</param>
        /// <returns>True if: (Maintenance OR Super team) OR (Customer team AND is team leader)</returns>
        public bool IsAuthorized(HttpContext context)
        {
            // Check if the user is in the Customer Team
            var isInCusTeam = context.User.IsInCustomerTeam();
            // Check if the user is in the Maintenance Team
            var isInMntcTeam = context.User.IsInMntcTeam();
            // Check if the user is in the Super Team
            var isInSprTeam = context.User.IsInSuperTeam();
            // Determine if the user is in a team higher than the Customer Team
            var isInHigherThanCustomerTeam = isInMntcTeam || isInSprTeam;

            // Check if the user is a team leader
            var isLeader = context.User.IsTeamLeader();

            // The user is authorized if they are in a higher team or if they are in the Customer Team and a leader
            return isInHigherThanCustomerTeam || isInCusTeam && isLeader;
        }
    }//Cls


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//


    #region FiltersAndPolicies

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
        public const string NAME = $"{nameof(CustomerLeaderMinimumAuthenticator)}.{nameof(Policy)}";

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


public static class CustomerLeaderMinimumPolicySetupExtensions
{
    public static IServiceCollection AddCustomerLeaderMinimumPolicy(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, CustomerLeaderMinimumAuthenticator.Policy.Handler>();
        return services.AddAuthorization(options =>
              options.AddPolicy(CustomerLeaderMinimumAuthenticator.Policy.NAME, policy =>
                  policy.Requirements.Add(new CustomerLeaderMinimumAuthenticator.Policy.Requirement()))
        );
    }
}//Cls



//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
//#-#-#-#-#-#      SETUP     #-#-#-#-#-#//
//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
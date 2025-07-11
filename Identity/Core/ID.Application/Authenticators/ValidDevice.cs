using ID.Application.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using StringHelpers;

namespace ID.Application.Authenticators;
public static class ValidDeviceAuthenticator
{
    /// <summary>
    /// User must have Device Id that matches one of the Devices on Subscription with name subscriptionName.
    /// </summary>
    protected internal class AuthHandler
    {
        /// <summary>
        /// <inheritdoc cref="AuthHandler"/>
        /// </summary>
        /// <param name="context">The HTTP context containing the user information.</param>
        /// <returns>True if the user is authorized; otherwise, false.</returns>
        public static bool IsAuthorized(HttpContext context, string subscriptionName)
        {
            var deviceId = context.GetDeviceId(subscriptionName);
            return !deviceId.IsNullOrWhiteSpace();
        }
    }//Cls


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//

    #region FiltersAndPolicies

    /// <summary>
    /// <inheritdoc cref="AuthHandler"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ResourceFilter(string subscriptionName) : Attribute, IAsyncResourceFilter
    {
        public async Task OnResourceExecutionAsync(
            ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (!context.HttpContext.IsAuthenticated())
                context.Result = new UnauthorizedResult();
            else if (AuthHandler.IsAuthorized(context.HttpContext, subscriptionName))
                await next();
            else
                context.Result = new ForbidResult();
        }
    }//Cls


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//


    /// <summary>ActionFilter
    /// Only a CustomerLeader or Mntc or Super team members shall pass!
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ActionFilter(string subscriptionName) : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.IsAuthenticated())
                context.Result = new UnauthorizedResult();
            else if (AuthHandler.IsAuthorized(context.HttpContext, subscriptionName))
                base.OnActionExecuting(context);
            else
                context.Result = new ForbidResult();
        }

    }//Cls


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//

    public static class Policy
    {
        public const string NAME = $"{nameof(ValidDeviceAuthenticator)}.{nameof(Policy)}";

        //------------------------------//

        public record Requirement(string SubscriptionName) : IAuthorizationRequirement { }

        //------------------------------//

        public class Handler(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<Requirement>
        {
            private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, Requirement requirement)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null || !AuthHandler.IsAuthorized(httpContext, requirement.SubscriptionName))
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


public static class ValidDeviceAuthenticatorPolicySetupExtensions
{
    public static IServiceCollection AddValidDeviceAuthenticatorPolicy(this IServiceCollection services, string subscriptionName)
    {
        services.AddSingleton<IAuthorizationHandler, ValidDeviceAuthenticator.Policy.Handler>();
        return services.AddAuthorization(options =>
              options.AddPolicy(ValidDeviceAuthenticator.Policy.NAME, policy =>
                  policy
                    .RequireAuthenticatedUser()
                    .Requirements.Add(new ValidDeviceAuthenticator.Policy.Requirement(subscriptionName)))
        );
    }
}//Cls


//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
//#-#-#-#-#-#      SETUP     #-#-#-#-#-#//
//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
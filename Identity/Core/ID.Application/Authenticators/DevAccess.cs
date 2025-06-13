using ID.Application.Authenticators.Abstractions;
using ID.Application.Authenticators.Utils;
using ID.Application.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ID.Application.Authenticators;
public static class DevAccessAuthenticator
{
    /// <summary>
    /// Only accessible in Development Environment
    /// </summary>
    protected internal class AuthHandler : IAuthHandler
    {
        /// <summary>
        /// <inheritdoc cref="AuthHandler"/>
        /// </summary>
        /// <param name="context">The HTTP context containing the user information.</param>
        /// <returns>True if the user is authorized; otherwise, false.</returns>
        public bool IsAuthorized(HttpContext context)
        {
            return context.IsDevEnv();
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
            var httpContext = context.HttpContext;

            if (httpContext.IsDevEnv())
                await next();
            else if (!httpContext.IsAuthenticated())
                context.Result = new UnauthorizedResult();
            else if (new AuthHandler().IsAuthorized(context.HttpContext))
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
            var httpContext = context.HttpContext;

            if (httpContext.IsDevEnv())
                base.OnActionExecuting(context);
            else if (!httpContext.IsAuthenticated())
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
        public const string NAME = $"{nameof(DevAccessAuthenticator)}.{nameof(Policy)}";

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

                if (httpContext == null)
                    context.Fail();
                else if (httpContext.IsDevEnv())
                    context.Fail();
                else if (!httpContext.IsAuthenticated())
                    context.Fail();
                else if (!baseHandler.IsAuthorized(httpContext))
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


public static class DevAccessAuthenticatorPolicySetupExtensions
{
    public static IServiceCollection AddDevAccessAuthenticatorPolicy(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, DevAccessAuthenticator.Policy.Handler>();
        return services.AddAuthorization(options =>
              options.AddPolicy(DevAccessAuthenticator.Policy.NAME, policy =>
                  policy
                    //.RequireAuthenticatedUser() //This is handled in the Handler
                    .Requirements.Add(new DevAccessAuthenticator.Policy.Requirement()))
        );
    }
}//Cls


//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
//#-#-#-#-#-#      SETUP     #-#-#-#-#-#//
//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
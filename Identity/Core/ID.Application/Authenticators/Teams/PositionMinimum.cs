using ID.Application.Authenticators.Abstractions;
using ID.Application.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Application.Authenticators.Teams;
public static class PositionMinimumAuthenticator
{

    /// <summary>
    ///
    /// </summary>
    protected internal class AuthHandler : IAuthPositionHandler
    {
        /// <summary>
        /// <inheritdoc cref="AuthHandler"/>
        /// </summary>
        public bool IsAuthorized(HttpContext context, int? level = -1) =>
            context.IsAuthenticated() &&  context.TeamPosition() >= (level ?? -1);

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
            if (!context.HttpContext.IsAuthenticated())
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
        public const string NAME = $"{nameof(MntcAuthenticator)}.{nameof(Policy)}";

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
                if (httpContext == null || !baseHandler.IsAuthorized(httpContext, requirement.Level))
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


public static class PositionMinimumAuthenticatorPolicySetupExtensions
{
    public static IServiceCollection AddPositionMinimumAuthenticatorPolicy(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, PositionMinimumAuthenticator.Policy.Handler>();
        return services.AddAuthorization(options =>
              options.AddPolicy(PositionMinimumAuthenticator.Policy.NAME, policy =>
                  policy
                    .RequireAuthenticatedUser()
                    .Requirements.Add(new PositionMinimumAuthenticator.Policy.Requirement(null)))
        );
    }
}//Cls


//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
//#-#-#-#-#-#      SETUP     #-#-#-#-#-#//
//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
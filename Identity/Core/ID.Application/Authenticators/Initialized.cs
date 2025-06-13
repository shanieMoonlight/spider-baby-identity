using ID.Application.Authenticators.Abstractions;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Application.Authenticators;
public static class InitializedAuthenticator
{
    /// <summary>
    /// Only lets you pass if the app has not been INitialized for Id yet.
    /// </summary>
    protected internal class AuthHandler : IAuthHandler
    {
        /// <summary>
        /// <inheritdoc cref="AuthHandler"/>
        /// </summary>
        /// <param name="context">The HTTP context containing the user information.</param>
        /// <returns>True if the user is authorized; otherwise, false.</returns>
        public bool IsAuthorized(HttpContext context) =>
            !IsAlreadyInitialized(context);

        //------------------------------------//

        public static bool IsAlreadyInitialized(HttpContext context)
        {
            try
            {
                var svc = context.RequestServices;
                var teamMgr = svc.GetService<IIdentityTeamManager<AppUser>>();
                var superTeam = teamMgr?.GetSuperTeamWithMembersAsync().Result;
                var superLeader = superTeam?.Members?
                    .FirstOrDefault(m => m.Id == superTeam.LeaderId);

                return superLeader is not null;
            }
            catch (Exception)
            {
                return false;
            }
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
            //This request won't be authenticated
            if (new AuthHandler().IsAuthorized(context.HttpContext))
                await next();
            else
                context.Result = new NotFoundResult();
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

            //This request won't be authenticated
            if (new AuthHandler().IsAuthorized(context.HttpContext))
                base.OnActionExecuting(context);
            else
                context.Result = new NotFoundResult();
        }

    }//Cls


    //#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#//

    public static class Policy
    {
        public const string NAME = $"{nameof(InitializedAuthenticator)}.{nameof(Policy)}";

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


public static class InitializedAuthenticatorPolicySetupExtensions
{
    public static IServiceCollection AddInitializedAuthenticatorPolicy(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, InitializedAuthenticator.Policy.Handler>();
        return services.AddAuthorization(options =>
              options.AddPolicy(InitializedAuthenticator.Policy.NAME, policy =>
                  policy
                    .Requirements.Add(new InitializedAuthenticator.Policy.Requirement()))
        );
    }
}//Cls


//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
//#-#-#-#-#-#      SETUP     #-#-#-#-#-#//
//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
using ID.Application.Utility.ExtensionMethods;
using ID.Domain.Claims;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Routes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;



namespace ID.Application.Middleware;

//#####################################################//

/// <summary>
/// If the principal has TwoFactorClaims.TwoFactorRequired and TwoFactorClaims.TwoFactor_NOT_Verified.
/// It can't access anything except the TwoFactorVerification, TwoFactorResend and Login actions.
/// </summary>
/// <param name="next"></param>
internal class MultiFactorRequiredMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (IsAttemptingToVerifyTwoFactor(context.Request))
        {
            await next(context);
            return;
        }

        var twoFactorRequired = context.User.HasClaim(TwoFactorClaims.TwoFactorRequired);
        var twoFactorNotVerified = context.User.HasClaim(TwoFactorClaims.TwoFactor_NOT_Verified);

        if (twoFactorRequired && twoFactorNotVerified)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(IDMsgs.Error.TwoFactor.MULTI_FACTOR_NOT_YET_VERIFIED);
            return;
        }

        await next(context);
    }

    //------------------------------------//

    private static bool IsAttemptingToVerifyTwoFactor(HttpRequest request)
    {
        var ctrl = $"{request.RouteValues["controller"]}";
        var action = $"{request.RouteValues["action"]}";
        var path = request.Path.Value ?? "";


        return IsCallingMfaVerify(path, ctrl)
            || IsCallingMfaResend(path, ctrl)
            || IsCallingLogin(path, ctrl)
            || IsCallingSignIn(path, ctrl);
    }

    //- - - - - - - - - - - - - - - - - - //

    private static bool IsCallingMfaVerify(string requestPath, string ctrl) =>
        ctrl.Equals(IdRoutes.Account.Controller, StringComparison.CurrentCultureIgnoreCase)
        &&
        (
            requestPath.Contains(IdRoutes.Account.Actions.TwoFactorVerification, StringComparison.CurrentCultureIgnoreCase)
            || 
            requestPath.Contains(IdRoutes.Account.Actions.TwoFactorVerificationCookie, StringComparison.CurrentCultureIgnoreCase)
        );

    //- - - - - - - - - - - - - - - - - - //

    private static bool IsCallingMfaResend(string requestPath, string ctrl) =>
        ctrl.Equals(IdRoutes.Account.Controller, StringComparison.CurrentCultureIgnoreCase)
        &&
        requestPath.Contains(IdRoutes.Account.Actions.TwoFactorResend, StringComparison.CurrentCultureIgnoreCase);


    //- - - - - - - - - - - - - - - - - - //

    private static bool IsCallingLogin(string requestPath, string ctrl) =>
        ctrl.Equals(IdRoutes.Account.Controller, StringComparison.CurrentCultureIgnoreCase)
        &&
        requestPath.Contains(IdRoutes.Account.Actions.Login, StringComparison.CurrentCultureIgnoreCase);


    //- - - - - - - - - - - - - - - - - - //

    private static bool IsCallingSignIn(string requestPath, string ctrl) =>
        ctrl.Equals(IdRoutes.Account.Controller, StringComparison.CurrentCultureIgnoreCase)
        &&
        requestPath.Contains(IdRoutes.Account.Actions.CookieSignIn, StringComparison.CurrentCultureIgnoreCase);


    //------------------------------------//
}


//#####################################################//


// Extension method to make it easy to add the middleware to the pipeline
internal static class MultiFactorRequiredMiddlewareExtensions
{
    public static IApplicationBuilder UseMultiFactorRequiredMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<MultiFactorRequiredMiddleware>();
}


//#####################################################//
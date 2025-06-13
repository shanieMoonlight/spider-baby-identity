using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Diagnostics;

namespace ID.Infrastructure.Auth.Cookies.Events;

/// <summary>
/// Cookie authentication events implementation that logs debug information
/// about authentication cookies.
/// </summary>
public class DebugCookieEvents : CookieAuthenticationEvents
{
    /// <summary>
    /// Called each time a request principal has been validated by the middleware.
    /// </summary>
    /// <param name="context">Contains information about the login session and the user.</param>
    /// <returns>A task representing the completed operation.</returns>
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var properties = context.Properties;

        Debug.WriteLine($"{nameof(properties.IssuedUtc)}:   {properties.IssuedUtc}");
        Debug.WriteLine($"{nameof(properties.ExpiresUtc)}:  {properties.ExpiresUtc}");
        Debug.WriteLine($"{nameof(properties.IsPersistent)}:{properties.IsPersistent}");
        Debug.WriteLine($"{nameof(properties.RedirectUri)}: {properties.RedirectUri}");
        Debug.WriteLine($"{nameof(properties.AllowRefresh)}:{properties.AllowRefresh}");

        await base.ValidatePrincipal(context);
    }


    //------------------------//


    /// <summary>
    /// Called when a sliding expiration token has passed its expiration window.
    /// </summary>
    /// <param name="context">Contains information about the sliding expiration.</param>
    /// <returns>A task representing the completed operation.</returns>
    public override async Task CheckSlidingExpiration(CookieSlidingExpirationContext context)
    {
        Debug.WriteLine($"*****************************");
        Debug.WriteLine($"{nameof(CheckSlidingExpiration)}");
        Debug.WriteLine($"*****************************");
        var properties = context.Properties;

        Debug.WriteLine($"{nameof(properties.IssuedUtc)}:   {properties.IssuedUtc}");
        Debug.WriteLine($"{nameof(properties.ExpiresUtc)}:  {properties.ExpiresUtc}");
        Debug.WriteLine($"{nameof(properties.IsPersistent)}:{properties.IsPersistent}");
        Debug.WriteLine($"{nameof(properties.RedirectUri)}: {properties.RedirectUri}");
        Debug.WriteLine($"{nameof(context.ElapsedTime)}:{context.ElapsedTime}");
        Debug.WriteLine($"{nameof(context.RemainingTime)}:{context.RemainingTime}");
        Debug.WriteLine($"{nameof(context.ShouldRenew)}:{context.ShouldRenew}");

        await base.CheckSlidingExpiration(context);
    }


    //------------------------//
 
    override public Task SigningIn(CookieSigningInContext context)
    {
        Debug.WriteLine($"*****************************");
        Debug.WriteLine($"{nameof(SigningIn)}");
        Debug.WriteLine($"*****************************");

        var properties = context.Properties;
        Debug.WriteLine($"{nameof(properties.IssuedUtc)}:   {properties.IssuedUtc}");
        Debug.WriteLine($"{nameof(properties.ExpiresUtc)}:  {properties.ExpiresUtc}");
        Debug.WriteLine($"{nameof(properties.IsPersistent)}:{properties.IsPersistent}");
        Debug.WriteLine($"{nameof(properties.RedirectUri)}: {properties.RedirectUri}");
        Debug.WriteLine($"{nameof(properties.AllowRefresh)}:{properties.AllowRefresh}");
        return base.SigningIn(context);
    }

    //------------------------//

    public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
    {
        Debug.WriteLine($"*****************************");
        Debug.WriteLine($"{nameof(RedirectToAccessDenied)}");
        Debug.WriteLine($"*****************************");

        var properties = context.Properties;
        Debug.WriteLine($"{nameof(properties.IssuedUtc)}:   {properties.IssuedUtc}");
        Debug.WriteLine($"{nameof(properties.ExpiresUtc)}:  {properties.ExpiresUtc}");
        Debug.WriteLine($"{nameof(properties.IsPersistent)}:{properties.IsPersistent}");
        Debug.WriteLine($"{nameof(properties.RedirectUri)}: {properties.RedirectUri}");
        Debug.WriteLine($"{nameof(properties.AllowRefresh)}:{properties.AllowRefresh}");
        Debug.WriteLine($"{nameof(context.Scheme)}:{context.Scheme}");
        Debug.WriteLine($"{nameof(context.Options)}:{context.Options}");
        return base.RedirectToAccessDenied(context);
    }

    //------------------------//

    public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        Debug.WriteLine($"*****************************");
        Debug.WriteLine($"{nameof(RedirectToLogin)}");
        Debug.WriteLine($"*****************************");

        var properties = context.Properties;
        Debug.WriteLine($"{nameof(properties.IssuedUtc)}:   {properties.IssuedUtc}");
        Debug.WriteLine($"{nameof(properties.ExpiresUtc)}:  {properties.ExpiresUtc}");
        Debug.WriteLine($"{nameof(properties.IsPersistent)}:{properties.IsPersistent}");
        Debug.WriteLine($"{nameof(properties.RedirectUri)}: {properties.RedirectUri}");
        Debug.WriteLine($"{nameof(properties.AllowRefresh)}:{properties.AllowRefresh}");
        Debug.WriteLine($"{nameof(context.RedirectUri)}:{context.RedirectUri}");
        Debug.WriteLine($"{nameof(context.Scheme)}:{context.Scheme}");
        Debug.WriteLine($"{nameof(context.Options)}:{context.Options}");
        return base.RedirectToLogin(context);
    }

    //------------------------//


    public override Task RedirectToLogout(RedirectContext<CookieAuthenticationOptions> context)
    {
        Debug.WriteLine($"*****************************");
        Debug.WriteLine($"{nameof(RedirectToLogout)}");
        Debug.WriteLine($"*****************************");

        var properties = context.Properties;
        Debug.WriteLine($"{nameof(properties.IssuedUtc)}:   {properties.IssuedUtc}");
        Debug.WriteLine($"{nameof(properties.ExpiresUtc)}:  {properties.ExpiresUtc}");
        Debug.WriteLine($"{nameof(properties.IsPersistent)}:{properties.IsPersistent}");
        Debug.WriteLine($"{nameof(properties.RedirectUri)}: {properties.RedirectUri}");
        Debug.WriteLine($"{nameof(properties.AllowRefresh)}:{properties.AllowRefresh}");
        Debug.WriteLine($"{nameof(context.RedirectUri)}:{context.RedirectUri}");
        Debug.WriteLine($"{nameof(context.Scheme)}:{context.Scheme}");
        Debug.WriteLine($"{nameof(context.Options)}:{context.Options}");
        return base.RedirectToLogout(context);
    }

    //------------------------//


    public override Task RedirectToReturnUrl(RedirectContext<CookieAuthenticationOptions> context)
    {
        Debug.WriteLine($"*****************************");
        Debug.WriteLine($"{nameof(RedirectToReturnUrl)}");
        Debug.WriteLine($"*****************************");

        var properties = context.Properties;
        Debug.WriteLine($"{nameof(properties.IssuedUtc)}:   {properties.IssuedUtc}");
        Debug.WriteLine($"{nameof(properties.ExpiresUtc)}:  {properties.ExpiresUtc}");
        Debug.WriteLine($"{nameof(properties.IsPersistent)}:{properties.IsPersistent}");
        Debug.WriteLine($"{nameof(properties.RedirectUri)}: {properties.RedirectUri}");
        Debug.WriteLine($"{nameof(properties.AllowRefresh)}:{properties.AllowRefresh}");
        Debug.WriteLine($"{nameof(context.RedirectUri)}:{context.RedirectUri}");
        Debug.WriteLine($"{nameof(context.Scheme)}:{context.Scheme}");
        Debug.WriteLine($"{nameof(context.Options)}:{context.Options}");
        return base.RedirectToReturnUrl(context);
    }

    //------------------------//

    public override Task SigningOut(CookieSigningOutContext context)
    {
        Debug.WriteLine($"*****************************");
        Debug.WriteLine($"{nameof(SigningOut)}");
        Debug.WriteLine($"*****************************");

        var properties = context.Properties;
        if (properties != null)
        {
            Debug.WriteLine($"{nameof(properties.IssuedUtc)}:   {properties.IssuedUtc}");
            Debug.WriteLine($"{nameof(properties.ExpiresUtc)}:  {properties.ExpiresUtc}");
            Debug.WriteLine($"{nameof(properties.IsPersistent)}:{properties.IsPersistent}");
            Debug.WriteLine($"{nameof(properties.RedirectUri)}: {properties.RedirectUri}");
            Debug.WriteLine($"{nameof(properties.AllowRefresh)}:{properties.AllowRefresh}");
        }
        else
        {
            Debug.WriteLine("Properties is null");
        }
        return base.SigningOut(context);
    }

    //------------------------//

    public override Task SignedIn(CookieSignedInContext context)
    {
        Debug.WriteLine($"*****************************");
        Debug.WriteLine($"{nameof(SignedIn)}");
        Debug.WriteLine($"*****************************");

        var properties = context.Properties;
        Debug.WriteLine($"{nameof(properties.IssuedUtc)}:   {properties.IssuedUtc}");
        Debug.WriteLine($"{nameof(properties.ExpiresUtc)}:  {properties.ExpiresUtc}");
        Debug.WriteLine($"{nameof(properties.IsPersistent)}:{properties.IsPersistent}");
        Debug.WriteLine($"{nameof(properties.RedirectUri)}: {properties.RedirectUri}");
        Debug.WriteLine($"{nameof(properties.AllowRefresh)}:{properties.AllowRefresh}");
        return base.SignedIn(context);
    }

    //------------------------//

}//Cls
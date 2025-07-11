﻿using Hangfire.Dashboard;


namespace ID.Infrastructure.Jobs.Service.HangFire.Auth;


/// <summary>
/// Make sure the user is authenticated before accessing the dashboard
/// </summary>
public class HangFire_Authenticated_AuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var user = httpContext.User;
        var isAuthenticated = user.Identity?.IsAuthenticated ?? false;

        return isAuthenticated;
    }
}//Cls

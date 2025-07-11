﻿using Hangfire.Dashboard;
using ID.Application.Utility.ExtensionMethods;

namespace ID.Infrastructure.Jobs.Service.HangFire.Auth;


/// <summary>
/// Make sure the user is authenticated before accessing the dashboard
/// And in Super Team
/// </summary>
public class HangFire_SuperMin_AuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var user = httpContext.User;
        var isInSuperTeam = user.IsInSuperTeam();
        
        return isInSuperTeam;
    }
}//Cls
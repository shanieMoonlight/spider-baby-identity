using Hangfire.Dashboard;
using ID.Application.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ID.Infrastructure.Jobs.Service.HangFire.Auth;


/// <summary>
/// Make sure the user is authenticated before accessing the dashboard
/// And in Mntc Team or Super Team
/// </summary>
public class HangFire_MntcMin_OrDev_AuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var environment = httpContext.RequestServices.GetService<IWebHostEnvironment>(); 
        var isDevelopment = environment?.IsDevelopment() ?? false;
        var user = httpContext.User;
        var isMntcMinimum = user.IsInMntcTeamMinimum();

        return isDevelopment || isMntcMinimum;
    }
}//Cls
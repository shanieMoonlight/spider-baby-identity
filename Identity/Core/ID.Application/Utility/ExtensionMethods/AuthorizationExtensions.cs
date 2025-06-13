using ID.GlobalSettings.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace ID.GlobalSettings.Utility.ExtensionMethods;

public static class AuthorizationExtensions
{

    public static void SetForbiddenResult(this AuthorizationFilterContext context, string msg) =>
        context.Result = new ObjectResult(msg)
        {
            StatusCode = (int)HttpStatusCode.Forbidden
        };

}//Cls
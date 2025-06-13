using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace LoggingHelpers.Principal;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserInfoToString(this HttpContext context)
    {
        if (context == null)
            return "";

        var user = context.User;

        var userId = user.GetLoggedInUserId();
        var userEmail = user.GetLoggedInUserEmail();
        var userName = user.GetLoggedInUserName();
        var otherClaims = user.GetAllUserClaims();

        StringBuilder messageBuilder = new();

        messageBuilder
           .AppendLine($"UserId:  {userId}")
           .AppendLine()
           .AppendLine($"UserName:  {userName}")
           .AppendLine()
           .AppendLine($"UserEmail:  {userEmail}")
           .AppendLine()
           .AppendLine($"{otherClaims}")
           .AppendLine();


        return messageBuilder.ToString();

    }

    //------------------------------------//

    public static string GetLoggedInUserId(this ClaimsPrincipal principal) =>
        principal?.GetClaimValue(ClaimTypes.NameIdentifier)
            ?? principal?.GetClaimValue(JwtRegisteredClaimNames.Sub)
            ?? "no-userId";

    //------------------------------------//

    public static string GetLoggedInUserName(this ClaimsPrincipal principal) =>
        principal?.GetClaimValue(ClaimTypes.Name) ?? "no-username";

    //------------------------------------//

    public static string GetLoggedInUserEmail(this ClaimsPrincipal principal) =>
        principal?.GetClaimValue(ClaimTypes.Email) ?? "no-email";

    //------------------------------------//

    public static string GetAllUserClaims(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var sb = new StringBuilder();

        foreach (var claim in principal.Claims)
        {
            sb.AppendLine($"{claim.Type}: {claim.Value}")
               .AppendLine();
        }

        return sb.ToString();

    }

    //------------------------------------//

    public static string? GetClaimValue(this ClaimsPrincipal principal, string claimType) =>
        principal?.FindFirst(claimType)?.Value;

    //------------------------------------//

    public static List<string>? GetClaimValues(this ClaimsPrincipal principal, string claimType) =>
        principal?.FindAll(claimType)?.Select(c => c.Value).ToList();


}//Cls
using ID.Application.AppAbs.RequestInfo;
using ID.Application.Utility.ExtensionMethods;
using ID.Domain.Claims.Utils;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ID.Application.AppImps.RequestInfo;

public class UserInfo(IHttpContextAccessor httpContextAccessor) : IUserInfo
{
    //------------------------------------//

    private const string _not_found = "SYSTEM";
    private readonly ClaimsPrincipal? _principal = httpContextAccessor?.HttpContext?.User;

    //------------------------------------//

    public string GetLoggedInUserId(string? fallback = null) =>
        _principal?.GetClaimValue(ClaimTypes.NameIdentifier) ?? fallback ?? _not_found;


    //------------------------------------//

    public string GetLoggedInUserName(string? fallback = null) =>
        _principal?.GetClaimValue(ClaimTypes.Name) 
        ?? _principal?.GetClaimValue(MyIdClaimTypes.NAME)
        ?? fallback 
        ?? _not_found;

    //------------------------------------//

    public string GetLoggedInUserEmail(string? fallback = null) =>
        _principal?.GetClaimValue(ClaimTypes.Email)
        ?? _principal?.GetClaimValue(MyIdClaimTypes.EMAIL) 
        ?? fallback 
        ?? _not_found;

    //------------------------------------//

    public string GetLoggedInNameAndEmail(string? fallback = null) =>
        $"{GetLoggedInUserName(fallback)} / {GetLoggedInUserEmail(fallback)}";


}//Cls

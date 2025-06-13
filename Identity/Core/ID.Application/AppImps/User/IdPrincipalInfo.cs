using ID.Application.AppAbs.ApplicationServices.Principal;
using ID.Application.Utility.ExtensionMethods;
using ID.Domain.Entities.Teams;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ID.Application.AppImps.User;

public class IdPrincipalInfo(IHttpContextAccessor httpContextAccessor) : IIdPrincipalInfo
{
    //------------------------------------//


    private readonly HttpContext? _ctx = httpContextAccessor?.HttpContext;
    private readonly ClaimsPrincipal? _principal = httpContextAccessor?.HttpContext?.User;

    //------------------------------------//

    public ClaimsPrincipal? GetPrincipal() => _principal;

    //------------------------------------//

    public bool IsAuthenticated() => _principal?.Identity?.IsAuthenticated ?? false;

    //------------------------------------//

    public bool IsMntc() => _principal.IsInMntcTeam();
    
    public bool IsSpr() => _principal.IsInSuperTeam();

    public bool IsCustomer() => _principal.IsInCustomerTeam();

    //------------------------------------//

    public bool IsCustomerMinimum() =>
        IsCustomer() || IsMntc() || IsSpr();

    public bool IsMntcMinimum() =>
        IsMntc() || IsSpr();

    public bool IsSprMinimum() => IsSpr();

    //------------------------------------//

    public Guid? TeamId() => _principal?.GetTeamId();

    public Guid? UserId() => _principal?.GetUserId();

    public int TeamPositionValue() => _principal?.TeamPosition() ?? -1;

    public bool IsLeader() => _principal?.IsTeamLeader() ?? false;

    //------------------------------------//

    public TeamType? TeamType() => _principal?.GetTeamType();

    public string? Email() => _principal?.GetEmail();

    public string? Username() => _principal?.GetUsername();

    public string? DeviceId(string subName) =>  _principal?.GetDeviceId(subName);

    //------------------------------------//

}//Cls

using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.Teams;
using System.Security.Claims;

namespace ID.Application.Mediatr.Cqrslmps;


public record APrincipalInfoRequest : IIdPrincipalInfoRequest
{
    public bool IsAuthenticated { get; set; }
    public Guid? PrincipalUserId { get; set; }
    public Guid? PrincipalTeamId { get; set; }
    public int PrincipalTeamPosition { get; set; }
    public string? PrincipalEmail { get; set; }
    public string? PrincipalUsername { get; set; }
    public bool IsSuper { get; set; }
    public bool IsSuperMinimum { get => IsSuper; }
    public bool IsSuperLeader { get => IsSuper && IsLeader; }
    public bool IsMntc { get; set; }
    public bool IsMntcMinimum { get => IsMntc || IsSuper; }
    public bool IsMntcLeader { get => IsMntc && IsLeader; }
    public bool IsCustomer { get; set; }
    public bool IsCustomerMinimum { get => IsCustomer || IsMntc || IsSuper; }
    public bool IsLeader { get; set; }
    public TeamType TeamType
    {
        get
        {
            if (IsMntc)
                return TeamType.maintenance;
            else if (IsSuper)
                return TeamType.super;
            else
                return TeamType.customer;
        }
    }
    public ClaimsPrincipal? Principal { get; set; }
    public string? UserIdentifier
    {
        get => PrincipalUsername
            ?? PrincipalEmail
            ?? PrincipalUserId?.ToString();
    }


}//Rcd

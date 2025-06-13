using ID.Domain.Entities.Teams;
using System.Security.Claims;

namespace ID.Application.Mediatr.CqrsAbs;

/// <summary>
/// This is the base interface for all MyID CQRS requests.
/// It will be populated by the Mediatr Pipeline so that all the user info will be available in Handlers and Validators
/// </summary>
public interface IIdPrincipalInfoRequest
{
    public Guid? PrincipalUserId { get; set; }
    public Guid? PrincipalTeamId { get; set; }
    public int PrincipalTeamPosition { get; set; }
    public string? PrincipalEmail { get; set; }
    public string? PrincipalUsername { get; set; }
    public bool IsAuthenticated { get; set; }
    public bool IsSuper { get; set; }
    public bool IsSuperMinimum { get; }
    public bool IsMntc { get; set; }
    public bool IsMntcMinimum { get; }
    public bool IsCustomer { get; set; }
    public bool IsCustomerMinimum { get; }
    public ClaimsPrincipal? Principal { get; set; }
    public string? UserIdentifier { get; }
    public TeamType TeamType { get; }
    public bool IsLeader { get; set; }
}
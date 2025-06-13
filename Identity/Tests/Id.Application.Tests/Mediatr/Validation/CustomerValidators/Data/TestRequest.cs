using System.Security.Claims;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.Teams;

namespace ID.Application.Tests.Mediatr.Validation.CustomerValidators.Data;


internal class TestRequest : IIdPrincipalInfoRequest
{
    public Guid? PrincipalUserId { get; set; }
    public Guid? PrincipalTeamId { get; set; }
    public int PrincipalTeamPosition { get; set; }
    public string? PrincipalEmail { get; set; }
    public string? PrincipalUsername { get; set; }
    public bool IsAuthenticated { get; set; }
    public bool IsSuper { get; set; }
    public bool IsSuperMinimum { get; set; }
    public bool IsMntc { get; set; }
    public bool IsMntcMinimum { get; set; }
    public bool IsCustomer { get; set; }
    public bool IsCustomerMinimum { get; set; }
    public ClaimsPrincipal? Principal { get; set; }
    public string? UserIdentifier { get; }

    public TeamType TeamType => TeamType.Customer;

    public bool IsLeader { get; set; }
}

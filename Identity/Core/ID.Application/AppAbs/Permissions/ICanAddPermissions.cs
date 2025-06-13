using ID.Application.Mediatr.CqrsAbs;
using MyResults;

namespace ID.Application.AppAbs.Permissions;
public interface ICanAddPermissions
{
    Task<BasicResult> CanAddCustomerTeamMember(int newMemberPosition, IIdPrincipalInfoRequest request);
    Task<BasicResult> CanAddTeamMember(int newMemberPosition, IIdPrincipalInfoRequest request);
}
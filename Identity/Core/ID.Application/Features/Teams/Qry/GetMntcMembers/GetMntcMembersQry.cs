using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.Teams.Qry.GetMntcMembers;
public record GetMntcMembersQry() : AIdQuery<IEnumerable<AppUserDto>>
{
}

using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.Teams.Qry.GetSprMembers;
public record GetSprMembersQry() : AIdQuery<IEnumerable<AppUserDto>>;

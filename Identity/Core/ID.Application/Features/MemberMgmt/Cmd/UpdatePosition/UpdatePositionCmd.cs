using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdatePosition;
public record UpdatePositionCmd(Guid UserId, int? NewPosition) : AIdUserAndTeamAwareCommand<AppUser, AppUserDto>;




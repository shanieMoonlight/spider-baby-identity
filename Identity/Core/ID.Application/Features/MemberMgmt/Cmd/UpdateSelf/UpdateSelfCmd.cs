using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdateSelf;
public record UpdateSelfCmd(UpdateSelfDto Dto) : AIdUserAndTeamAwareCommand<AppUser, AppUserDto>;






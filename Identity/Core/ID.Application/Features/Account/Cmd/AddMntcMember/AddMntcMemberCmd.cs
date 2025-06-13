using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.AddMntcMember;
public record AddMntcMemberCmd(AddMntcMemberDto Dto)
    : AIdUserAwareCommand<AppUser, AppUserDto>;
//Not using AIdUserAndTeamAwareCommand here because the SuperTeam might need to add a User to this team in some rare cases




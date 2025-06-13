using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.AddSprMember;

/// <summary>
/// Use for adding a new Super Team member
/// Can only be accessed by Suepr team members
/// </summary>
/// <param name="Dto">New member Data</param>
public record AddSprMemberCmd(AddSprMemberDto Dto) : AIdUserAndTeamAwareCommand<AppUser, AppUserDto>;




using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdateAddress;
public record UpdateAddressCmd(IdentityAddressDto Dto) : AIdUserAndTeamAwareCommand<AppUser, AppUserDto>;






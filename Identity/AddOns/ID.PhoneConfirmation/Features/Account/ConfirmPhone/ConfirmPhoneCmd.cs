using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.PhoneConfirmation.Features.Account.ConfirmPhone;
public record ConfirmPhoneCmd(ConfirmPhoneDto Dto) : AIdUserAndTeamAwareCommand<AppUser>;




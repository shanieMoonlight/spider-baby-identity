using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.PhoneConfirmation.Features.Account.ResendPhoneConfirmationPrincipal;
public record ResendPhoneConfirmationPrincipalCmd() : AIdUserAwareCommand<AppUser>;



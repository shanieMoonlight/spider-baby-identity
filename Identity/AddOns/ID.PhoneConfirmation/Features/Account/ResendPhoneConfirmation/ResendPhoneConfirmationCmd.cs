using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.PhoneConfirmation.Features.Account.ResendPhoneConfirmation;

//Using AIdUserAwareCommand to allow other team members to Resend Phone Confirmation. In case the user canm't login.
//Non team members will be unauthorized
public record ResendPhoneConfirmationCmd(ResendPhoneConfirmationDto Dto) : AIdUserAndTeamAwareCommand<AppUser>;




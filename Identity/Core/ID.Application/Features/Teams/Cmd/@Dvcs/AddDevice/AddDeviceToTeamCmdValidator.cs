
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Teams.Cmd.Dvcs.AddDevice;
public class AddDeviceToTeamCmdValidator : IsAuthenticatedValidator<AddDeviceToTeamCmd>
{
    public AddDeviceToTeamCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.SubscriptionId)
                .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

            RuleFor(p => p.Dto.Name)
               .NotEmpty()
               .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

            RuleFor(p => p.Dto.UniqueId)
               .NotEmpty()
               .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
        });

    }
}//Cls



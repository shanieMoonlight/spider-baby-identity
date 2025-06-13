
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Teams.Cmd.Create;
public class CreateCustomerTeamCmdValidator : AMntcMinimumValidator<CreateCustomerTeamCmd>
{
    public CreateCustomerTeamCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);


        RuleFor(p => p.Dto.Name)
            .NotEmpty()
                .When(m => m.Dto is not null)
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));


    }
}//Cls


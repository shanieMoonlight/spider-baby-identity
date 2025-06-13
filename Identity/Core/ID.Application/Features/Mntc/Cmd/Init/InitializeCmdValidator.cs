
using FluentValidation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Mntc.Cmd.Init;
public class InitializeCmdValidator : AbstractValidator<InitializeCmd>
{
    public InitializeCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                    .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.Password)
            .NotEmpty()
                    .WithMessage(IDMsgs.Error.IsRequired("Initialziation Password"));

        });

    }
}//Cls


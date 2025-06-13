
using FluentValidation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Customers.Features.Account.Cmd.RegCustomer;
public class RegCustomerCmdValidator : AbstractValidator<RegisterCustomerCmd>
{
    public RegCustomerCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.Email)
                .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));


            RuleFor(p => p.Dto.Password)
                .NotEmpty()
                        .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));


            RuleFor(p => p.Dto.ConfirmPassword)
                .NotEmpty()
                        .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));


            RuleFor(p => p.Dto.ConfirmPassword).Equal(p => p.Dto.Password);

        });
    }

}//Cls


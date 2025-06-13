
using FluentValidation;
using ID.Application.Mediatr.Behaviours.Validation;
using ID.Application.Mediatr.CqrsAbs;
using Microsoft.AspNetCore.Hosting;

namespace ID.Application.Mediatr.Validation;

//==============================================================//

public abstract class IsAuthenticatedValidator<TRequest>
    : AbstractValidator<TRequest> where TRequest
    : class, IIdPrincipalInfoRequest
{
    public IsAuthenticatedValidator()
    {
        RuleFor(p => p.IsAuthenticated)
          .NotEqual(false)
              .WithMessage("Unauthorized!")
              .WithState(state => ValidationError.Unauthorized);
    }

}

//==============================================================//

public abstract class IsAuthenticatedOrDevValidator<TRequest>(IWebHostEnvironment env)
    : ADevModeValidator<TRequest>(env) where TRequest
    : class, IIdPrincipalInfoRequest
{
    protected override void ConfigureRules()
    {
        RuleFor(p => p.IsAuthenticated)
         .NotEqual(false)
             .WithMessage("Unauthorized!")
             .WithState(state => ValidationError.Unauthorized);

        AdditionalRules();
    }


    protected abstract void AdditionalRules();

}

//==============================================================//


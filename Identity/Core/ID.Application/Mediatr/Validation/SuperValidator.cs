using FluentValidation;
using ID.Application.Mediatr.Behaviours.Validation;
using ID.Application.Mediatr.CqrsAbs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ID.Application.Mediatr.Validation;

//==============================================================//

/// <summary>
/// Base class for validation before handlers that should be accessed by Super Team Members only
/// </summary>
/// <typeparam name="TRequest">The Mediatr Request</typeparam>
public abstract class ASuperOnlyValidator<TRequest>
    : IsAuthenticatedValidator<TRequest> where TRequest : class, IIdPrincipalInfoRequest
{
    public ASuperOnlyValidator()
    {
        When(p => p.IsAuthenticated, () =>
        {
            RuleFor(p => p.IsSuper)
           .NotEqual(false)
               .WithMessage("Forbidden!")
               .WithState(state => ValidationError.Forbidden);
        });
    }

}

//= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = //

/// <summary>
/// Base class for validation before handlers that should be accessed by Super Team Members only
/// </summary>
/// <typeparam name="TRequest">The Mediatr Request</typeparam>
public abstract class ASuperOnlyOrDevValidator<TRequest>(IWebHostEnvironment env)
    : IsAuthenticatedOrDevValidator<TRequest>(env) where TRequest : class, IIdPrincipalInfoRequest
{

    protected override void AdditionalRules()
    {
        When(p => p.IsAuthenticated, () =>
        {
            RuleFor(p => p.IsSuper)
           .NotEqual(false)
               .WithMessage("Forbidden!")
               .WithState(state => ValidationError.Forbidden);
        });
    }

}

//==============================================================//

/// <summary>
/// Base class for validation before handlers that should be accessed by Super Team Members or Higher 
/// </summary>
/// <typeparam name="TRequest">The Mediatr Request</typeparam>
public abstract class ASuperMinimumValidator<TRequest>
    : AbstractValidator<TRequest> where TRequest : class, IIdPrincipalInfoRequest
{

    public ASuperMinimumValidator()
    {
        When(p => p.IsAuthenticated, () =>
        {
            RuleFor(p => p.IsSuperMinimum)
           .NotEqual(false)
               .WithMessage("Forbidden!")
               .WithState(state => ValidationError.Forbidden);
        });
    }

    public ASuperMinimumValidator(int position) : this()
    {
        When(p => p.IsSuperMinimum, () =>
        {
            RuleFor(p => p.PrincipalTeamPosition)
               .GreaterThanOrEqualTo(position)
                    .WithMessage("Forbidden!")
                   .WithState(state => ValidationError.Forbidden);
        });
    }
}

//= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = //


/// <summary>
/// Base class for validation before handlers that should be accessed by Super Team Members or Higher 
/// Unless we're in Dev Mode
/// </summary>
/// <typeparam name="TRequest">The Mediatr Request</typeparam>
public abstract class ASuperMinimumOrDevValidator<TRequest>(IWebHostEnvironment env, int? _position = null)
    : IsAuthenticatedOrDevValidator<TRequest>(env) where TRequest : class, IIdPrincipalInfoRequest
{
    protected override void AdditionalRules()
    {
        if (!_position.HasValue)
        {
            When(p => p.IsAuthenticated, () =>
            {
                RuleFor(p => p.IsSuperMinimum)
               .NotEqual(false)
                   .WithMessage("Forbidden!")
                   .WithState(state => ValidationError.Forbidden);
            });
        }
        else
        {
            When(p => p.IsSuperMinimum, () =>
            {
                RuleFor(p => p.PrincipalTeamPosition)
                   .GreaterThanOrEqualTo(_position.Value)
                       .WithMessage("Forbidden!")
                       .WithState(state => ValidationError.Forbidden);
            });
        }
    }

}

//==============================================================//
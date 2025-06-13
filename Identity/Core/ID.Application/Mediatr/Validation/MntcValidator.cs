using FluentValidation;
using ID.Application.Mediatr.Behaviours.Validation;
using ID.Application.Mediatr.CqrsAbs;
using Microsoft.AspNetCore.Hosting;

namespace ID.Application.Mediatr.Validation;

//==============================================================//

/// <summary>
/// Base class for validation before handlers that should be accessed by Mntc Team Members only
/// </summary>
/// <typeparam name="TRequest">The Mediatr Request</typeparam>
public abstract class AMntcOnlyValidator<TRequest>
    : IsAuthenticatedValidator<TRequest> where TRequest : class, IIdPrincipalInfoRequest
{
    public AMntcOnlyValidator()
    {
        When(p => p.IsAuthenticated, () =>
        {
            RuleFor(p => p.IsMntc)
           .NotEqual(false)
               .WithMessage("Forbidden!")
               .WithState(state => ValidationError.Forbidden);
        });
    }

    public AMntcOnlyValidator(int position) : this()
    {
        When(p => p.IsAuthenticated, () =>
        {
            RuleFor(p => p.PrincipalTeamPosition)
               .GreaterThanOrEqualTo(position)
                   .WithMessage("Forbidden!")
                   .WithState(state => ValidationError.Forbidden);
        });
    }
}

//==============================================================//


/// <summary>
/// Base class for validation before handlers that should be accessed by Mntc Team Members or Higher 
/// </summary>
/// <typeparam name="TRequest">The Mediatr Request</typeparam>
public abstract class AMntcMinimumValidator<TRequest>
    : IsAuthenticatedValidator<TRequest> where TRequest : class, IIdPrincipalInfoRequest
{

    public AMntcMinimumValidator()
    {
        When(p => p.IsAuthenticated, () =>
        {
            RuleFor(p => p.IsMntcMinimum)
           .NotEqual(false)
               .WithMessage("Forbidden!")
               .WithState(state => ValidationError.Forbidden);
        });
    }

    public AMntcMinimumValidator(int position) : this()
    {
        When(p => p.IsMntcMinimum, () =>
        {
            RuleFor(p => p.PrincipalTeamPosition)
               .GreaterThanOrEqualTo(position)
                   .WithMessage("Forbidden!")
                   .WithState(state => ValidationError.Forbidden);
        });
    }

}

//==============================================================//

/// <summary>
/// Base class for validation before handlers that should be accessed by Mntc Team Members only
/// Unless we're in Dev Mode
/// </summary>
/// <typeparam name="TRequest">The Mediatr Request</typeparam>
public abstract class AMntcOnlyOrDevValidator<TRequest>(IWebHostEnvironment env, int? _position = null)
    : IsAuthenticatedOrDevValidator<TRequest>(env) where TRequest : class, IIdPrincipalInfoRequest
{
    protected override void AdditionalRules()
    {
        if (!_position.HasValue)
        {
            When(p => p.IsAuthenticated, () =>
            {
                RuleFor(p => p.IsMntc)
               .NotEqual(false)
                   .WithMessage("Forbidden!")
                   .WithState(state => ValidationError.Forbidden);
            });
        }
        else
        {

            When(p => p.IsAuthenticated, () =>
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


/// <summary>
/// Base class for validation before handlers that should be accessed by Mntc Team Members or Higher 
/// Unless we're in Dev Mode
/// </summary>
/// <typeparam name="TRequest">The Mediatr Request</typeparam>
public abstract class AMntcMinimumOrDevValidator<TRequest>(IWebHostEnvironment env, int? _position = null)
    : IsAuthenticatedOrDevValidator<TRequest>(env) where TRequest : class, IIdPrincipalInfoRequest
{

    protected override void AdditionalRules()
    {
        if (!_position.HasValue)
        {
            When(p => p.IsAuthenticated, () =>
            {
                RuleFor(p => p.IsMntcMinimum)
               .NotEqual(false)
                   .WithMessage("Forbidden!")
                   .WithState(state => ValidationError.Forbidden);
            });
        }
        else
        {
            When(p => p.IsMntcMinimum, () =>
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
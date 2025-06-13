
using FluentValidation;
using ID.Application.Mediatr.Behaviours.Validation;
using ID.Application.Mediatr.CqrsAbs;
using ID.Application.Mediatr.Validation;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using System.Diagnostics;

namespace ID.Application.Customers.Mediatr.Validation;

//==============================================================//

/// <summary>
/// Base class for validation before handlers that should be accessed by Customer Team Members only
/// </summary>
/// <typeparam name="TRequest">The Mediatr Request</typeparam>
public abstract class ACustomerOnlyValidator<TRequest>
    : IsAuthenticatedValidator<TRequest>
    where TRequest : class, IIdPrincipalInfoRequest
{
    public ACustomerOnlyValidator()
    {
        When(p => p.IsAuthenticated, () =>
        {
            RuleFor(p => p.IsCustomer)
               .NotEqual(false)
                   .WithMessage("Forbidden!")
                   .WithState(state => ValidationError.Forbidden);
        });
    }

}

//==============================================================//


/// <summary>
/// Base class for validation before handlers that should be accessed by Customer Team Members or Higher 
/// </summary>
/// <typeparam name="TRequest">The Mediatr Request</typeparam>
public abstract class ACustomerMinimumValidator<TRequest>
    : IsAuthenticatedValidator<TRequest>
    where TRequest : class, IIdPrincipalInfoRequest
{

    public ACustomerMinimumValidator()
    {
        Debug.WriteLine($"Testing VAlidator");
        When(p => p.IsAuthenticated, () =>
        {
            RuleFor(p => p.IsCustomerMinimum)
           .NotEqual(false)
               .WithMessage("Forbidden!")
               .WithState(state => ValidationError.Forbidden);
        });
    }

    public ACustomerMinimumValidator(int position) : this()
    {
        When(p => p.IsCustomerMinimum, () =>
        {
            RuleFor(p => p.PrincipalTeamPosition)
               .GreaterThanOrEqualTo(position)
                    .WithMessage("Forbidden!")
                   .WithState(state => ValidationError.Forbidden);
        });
    }

}

//==============================================================//

public abstract class ACustomerLeaderValidator<TRequest>
    : AbstractValidator<TRequest> where TRequest
    : class, IIdPrincipalInfoRequest
{
    public ACustomerLeaderValidator()
    {
        RuleFor(p => p.IsLeader)
          .NotEqual(false)
              .WithMessage(IDMsgs.Error.Teams.ONlY_LEADER_CAN_UPDATE)
              .WithState(state => ValidationError.Forbidden);

        RuleFor(p => p.IsCustomer)
          .NotEqual(false)
              .WithMessage(IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(TeamType.Customer))
              .WithState(state => ValidationError.Forbidden);
    }

}


//##############################################################//
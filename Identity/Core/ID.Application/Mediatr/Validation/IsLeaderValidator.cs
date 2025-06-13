
using FluentValidation;
using ID.Application.Mediatr.Behaviours.Validation;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.Teams;

namespace ID.Application.Mediatr.Validation;

//##############################################################//

public abstract class IsLeaderValidator<TRequest>
    : AbstractValidator<TRequest> where TRequest
    : class, IIdPrincipalInfoRequest
{
    public IsLeaderValidator()
    {
        RuleFor(p => p.IsLeader)
          .NotEqual(false)
              .WithMessage(IDMsgs.Error.Teams.ONlY_LEADER_CAN_UPDATE)
              .WithState(state => ValidationError.Forbidden);
    }

}

//##############################################################//

public abstract class IsMntcLeaderValidator<TRequest>
    : AbstractValidator<TRequest> where TRequest
    : class, IIdPrincipalInfoRequest
{
    public IsMntcLeaderValidator()
    {
        RuleFor(p => p.IsLeader)
          .NotEqual(false)
              .WithMessage(IDMsgs.Error.Teams.ONlY_LEADER_CAN_UPDATE)
              .WithState(state => ValidationError.Forbidden);

        RuleFor(p => p.IsMntc)
          .NotEqual(false)
              .WithMessage(IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(TeamType.Maintenance))
              .WithState(state => ValidationError.Forbidden);
    }

}

public abstract class IsMntcMinimumLeaderValidator<TRequest>
    : AbstractValidator<TRequest> where TRequest
    : class, IIdPrincipalInfoRequest
{
    public IsMntcMinimumLeaderValidator()
    {
        RuleFor(p => p.IsLeader)
         .NotEqual(false)
             .WithMessage(IDMsgs.Error.Teams.ONlY_LEADER_CAN_UPDATE)
             .WithState(state => ValidationError.Forbidden);

        RuleFor(p => p.IsMntcMinimum)
          .NotEqual(false)
              .WithMessage(IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(TeamType.Maintenance))
              .WithState(state => ValidationError.Forbidden);
    }
}

//##############################################################//

public abstract class IsSuperLeaderValidator<TRequest>
    : AbstractValidator<TRequest> where TRequest
    : class, IIdPrincipalInfoRequest
{
    public IsSuperLeaderValidator()
    {
        RuleFor(p => p.IsLeader)
          .NotEqual(false)
              .WithMessage(IDMsgs.Error.Teams.ONlY_LEADER_CAN_UPDATE)
              .WithState(state => ValidationError.Forbidden);

        RuleFor(p => p.IsSuper)
          .NotEqual(false)
              .WithMessage(IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(TeamType.Super))
              .WithState(state => ValidationError.Forbidden);
    }

}



public abstract class IsSuperMinimumLeaderValidator<TRequest>
    : AbstractValidator<TRequest> where TRequest
    : class, IIdPrincipalInfoRequest
{
    public IsSuperMinimumLeaderValidator()
    {
        RuleFor(p => p.IsLeader)
         .NotEqual(false)
             .WithMessage(IDMsgs.Error.Teams.ONlY_LEADER_CAN_UPDATE)
             .WithState(state => ValidationError.Forbidden);

        RuleFor(p => p.IsSuperMinimum)
          .NotEqual(false)
              .WithMessage(IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(TeamType.Super))
              .WithState(state => ValidationError.Forbidden);
    }
}


//##############################################################//
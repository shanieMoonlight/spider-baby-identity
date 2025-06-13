using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.FeatureFlags.Qry.GetById;
public class GetFeatureFlagByIdQryValidator : AMntcMinimumValidator<GetFeatureFlagByIdQry>
{
    public GetFeatureFlagByIdQryValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

    }

}

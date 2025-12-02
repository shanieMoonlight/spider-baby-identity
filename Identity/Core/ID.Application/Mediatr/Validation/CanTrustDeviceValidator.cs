using ID.Application.Mediatr.Behaviours.Validation;
using ID.Application.Utility.ExtensionMethods;
using ID.Domain.Claims.AuthMethods;
using ID.GlobalSettings.Constants;

namespace ID.Application.Mediatr.Validation;


public abstract class CanTrustDeviceValidator<TRequest>
    : AbstractValidator<TRequest> where TRequest
    : class, IIdPrincipalInfoRequest
{
    public CanTrustDeviceValidator()
    {
        RuleFor(p => p.IsAuthenticated)
            .NotEqual(false)
            .WithMessage("Unauthorized!")
            .WithState(state => ValidationError.Unauthorized);

        When(p => p.IsAuthenticated, () =>
        {
            RuleFor(p => p.Principal.GetAuthMethodClaimValues()
                .Any(amr => amr == AuthMethodRef.mfa || amr == AuthMethodRef.oauth))
                .Equal(true)
                .WithMessage("You must use MFA or OAuth before you can add a Trusted Device.")
                .WithState(state => ValidationError.Unauthorized);


            RuleFor(p => p.Principal.GetAuthTime())
                .Must(at => at.HasValue && at.Value.AddMinutes(IdGlobalConstants.Authentication.MAX_AUTH_TIME_FOR_DEVICE_TRUST_MINUTES) >= DateTime.UtcNow)
                .WithMessage($"You must have logged in within the last {IdGlobalConstants.Authentication.MAX_AUTH_TIME_FOR_DEVICE_TRUST_MINUTES} minutes.")
                .WithState(state => ValidationError.Unauthorized);
        });
    }
}



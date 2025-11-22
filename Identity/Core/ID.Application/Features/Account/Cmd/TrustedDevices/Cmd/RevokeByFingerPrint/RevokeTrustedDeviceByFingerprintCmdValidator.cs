using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.RevokeByFingerPrint;
public class RevokeTrustedDeviceByFingerprintCmdValidator : IsAuthenticatedValidator<RevokeTrustedDeviceByFingerprintCmd>
{
    public RevokeTrustedDeviceByFingerprintCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.DeviceFingerprint)
              .NotEmpty()
                      .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
        });

    }

}


namespace ID.Application.Features.Account.TrustedDevices.Cmd.Trust;
public class TrustDeviceCmdValidator : IsAuthenticatedValidator<TrustDeviceCmd>
{
    public TrustDeviceCmdValidator()
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


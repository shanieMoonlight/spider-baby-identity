namespace ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetByFingerprint;
public class GetByFingerprintQryValidator : IsAuthenticatedValidator<GetTrustedDeviceByFingerprintQry>
{
    public GetByFingerprintQryValidator()
    {
        RuleFor(p => p.DeviceFingerprint)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

    }

}


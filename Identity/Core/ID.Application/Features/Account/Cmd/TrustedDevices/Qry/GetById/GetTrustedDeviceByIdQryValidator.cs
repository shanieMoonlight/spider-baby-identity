namespace ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetById;
public class GetTrustedDeviceByIdQryValidator : IsAuthenticatedValidator<GetTrustedDeviceByIdQry>
{
    public GetTrustedDeviceByIdQryValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

    }

}


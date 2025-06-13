using ID.Application.Mediatr.Validation;

namespace ID.Application.Features.Account.Cmd.RefreshTokenRevoke;
public class RefreshTokenRevokeCmdValidator
    : IsAuthenticatedValidator<RefreshTokenRevokeCmd>
{ }


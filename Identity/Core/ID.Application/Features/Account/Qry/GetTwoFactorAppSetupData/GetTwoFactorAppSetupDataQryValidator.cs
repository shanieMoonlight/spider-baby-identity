using ID.Application.Mediatr.Validation;

namespace ID.Application.Features.Account.Qry.GetTwoFactorAppSetupData;

public class GetTwoFactorAppSetupDataQryValidator() : IsAuthenticatedValidator<GetTwoFactorAppSetupDataQry>() { }

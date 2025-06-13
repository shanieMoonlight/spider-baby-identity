using ID.Application.Mediatr.Validation;

namespace ID.Application.Features.Account.Qry.MyInfo;
public class MyInfoQryValidator() : IsAuthenticatedValidator<MyInfoQry>() { }

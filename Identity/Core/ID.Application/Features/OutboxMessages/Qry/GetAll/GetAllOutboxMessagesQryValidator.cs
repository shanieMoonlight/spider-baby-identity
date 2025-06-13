using ID.Application.Mediatr.Validation;

namespace ID.Application.Features.OutboxMessages.Qry.GetAll;
public class GetAllOutboxMessagesQryValidator() : AMntcMinimumValidator<GetAllOutboxMessagesQry>(1) { }

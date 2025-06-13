using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.Testing.ConfirmationEmailById;
public record ConfirmationEmailByIdTestQry(Guid Id) : AIdQuery;

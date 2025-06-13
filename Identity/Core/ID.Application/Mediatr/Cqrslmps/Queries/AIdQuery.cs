using ID.Application.Mediatr.CqrsAbs;
using ID.Application.Mediatr.Cqrslmps;

namespace ID.Application.Mediatr.Cqrslmps.Queries;


public abstract record AIdQuery : APrincipalInfoRequest, IIdQuery;
public abstract record AIdQuery<TResponse> : APrincipalInfoRequest, IIdQuery<TResponse>;




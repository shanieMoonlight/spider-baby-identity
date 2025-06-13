using ID.Application.Mediatr.CqrsAbs;
using ID.Application.Mediatr.Cqrslmps;

namespace ID.Application.Mediatr.Cqrslmps.Commands;

/// <summary>
/// Used for tagging Mediatr Command requests that require a PrincipalInfoRequest (Uyser information)
/// APrincipalInfoRequest Values will be set in the pipeline. If not Request will short circuit  with NotFound or Unauthorized. So they will not be null in the handler
/// </summary>
public abstract record AIdCommand : APrincipalInfoRequest, IIdCommand;

/// <summary>
/// Used for tagging Mediatr Command requests that require a PrincipalInfoRequest (Uyser information)
/// APrincipalInfoRequest Values will be set in the pipeline. If not Request will short circuit  with NotFound or Unauthorized. So they will not be null in the handler
/// </summary>
public abstract record AIdCommand<TResponse> : APrincipalInfoRequest, IIdCommand<TResponse>;


using ID.Application.AppAbs.ApplicationServices.Principal;
using ID.Application.Mediatr.Auth;
using MyResults;

namespace ID.Application.Mediatr.Auth.Spr;
public sealed class SuperMinimumGuardBehaviour<TRequest, TResponse>(IIdPrincipalInfo userInfo)
    : AAuthGuardBehaviour<TRequest, TResponse, ISuperMinimumGuard>
    where TRequest : notnull
    where TResponse : BasicResult
{
    protected override bool Authenticate() => userInfo.IsSprMinimum();
}
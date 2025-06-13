using ID.Application.AppAbs.ApplicationServices.Principal;
using MyResults;

namespace ID.Application.Mediatr.Auth.Mntc;
public sealed class MntcMinimumGuardBehaviour<TRequest, TResponse>(IIdPrincipalInfo userInfo)
    : AAuthGuardBehaviour<TRequest, TResponse, IMntcMinimumGuard>
    where TRequest : notnull
    where TResponse : BasicResult
{
    protected override bool Authenticate() => userInfo.IsMntcMinimum();


}
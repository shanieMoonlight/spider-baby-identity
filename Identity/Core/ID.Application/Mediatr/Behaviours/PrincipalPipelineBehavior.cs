using ID.Application.AppAbs.ApplicationServices.Principal;
using ID.Application.Mediatr.CqrsAbs;
using MediatR;
using MyResults;

namespace ID.Application.Mediatr.Behaviours;


/// <summary>
/// Grabs all the auth info and attach it to the request
/// </summary>
public class IdPrincipalPipelineBehavior<TRequest, TResponse>(IIdPrincipalInfo userInfo)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdPrincipalInfoRequest, IRequest<TResponse>
    where TResponse : BasicResult//, new()
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        request.IsAuthenticated = userInfo.IsAuthenticated();
        request.PrincipalUserId = userInfo.UserId();
        request.PrincipalTeamId = userInfo.TeamId();
        request.PrincipalTeamPosition = userInfo.TeamPositionValue();
        request.PrincipalEmail = userInfo.Email();
        request.PrincipalUsername = userInfo.Username();
        request.IsSuper = userInfo.IsSpr();
        request.IsMntc = userInfo.IsMntc();
        request.IsCustomer = userInfo.IsCustomer();
        request.Principal = userInfo.GetPrincipal();
        request.IsLeader = userInfo.IsLeader();

        return await next(cancellationToken);
    }


}//Cls
using ID.Application.Mediatr.Behaviours.Common;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using MediatR;
using MyResults;

namespace ID.Application.Mediatr.Behaviours;


/// <summary>
/// Pipeline behavior that automatically loads user details and attaches them to user-aware requests.
/// </summary>
public class IdUserAwarePipelineBehavior<TRequest, TResponse>(IIdentityTeamManager<AppUser> _teamMgr)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdUserAwareRequest<AppUser>, IRequest<TResponse>
    where TResponse : BasicResult//, new()
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var user = await _teamMgr.GetMemberAsync(request.PrincipalTeamId, request.PrincipalUserId);
        if (user is null)
            return ResponseProvider.GenerateUnauthorizedUserResponse<TResponse>(request.UserIdentifier);


        request.PrincipalUser = user;

        return await next(cancellationToken);
    }


}//Cls
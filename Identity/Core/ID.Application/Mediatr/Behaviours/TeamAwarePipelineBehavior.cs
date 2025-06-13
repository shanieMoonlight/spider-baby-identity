using ID.Application.Mediatr.Behaviours.Common;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using MediatR;
using MyResults;

namespace ID.Application.Mediatr.Behaviours;


/// <summary>
/// Pipeline behavior that automatically loads team details and attaches them to team-aware requests.
/// </summary>
public class IdTeamAwarePipelineBehavior<TRequest, TResponse>(IIdentityTeamManager<AppUser> _teamMgr)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdUserAndTeamAwareRequest<AppUser>, IRequest<TResponse>
    where TResponse : BasicResult//, new()
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var team = await _teamMgr.GetByIdWithEverythingAsync(request.PrincipalTeamId, request.PrincipalTeamPosition);
        if (team is null)
            return ResponseProvider.GenerateNotFoundResponse<TResponse, Team>(request.PrincipalTeamId.ToString());

        request.SetTeam(team);

        return await next(cancellationToken);
    }


}//Cls

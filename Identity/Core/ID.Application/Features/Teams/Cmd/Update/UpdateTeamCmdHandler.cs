using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.Application.Features.Teams.Cmd.Update;
public class UpdateTeamCmdHandler(IIdentityTeamManager<AppUser> teamMgr) : IIdCommandHandler<UpdateTeamCmd, TeamDto>
{

    public async Task<GenResult<TeamDto>> Handle(UpdateTeamCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var team = request.PrincipalTeam;

        team.Update(dto);

        var entity = await teamMgr.UpdateAsync(team);

        return GenResult<TeamDto>.Success(entity!.ToDto());
    }


}//Cls





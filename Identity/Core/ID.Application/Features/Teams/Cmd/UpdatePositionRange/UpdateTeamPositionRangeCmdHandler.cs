using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams.ValueObjects;
using ID.Domain.Entities.Teams.Validators;
using MyResults;

namespace ID.Application.Features.Teams.Cmd.UpdatePositionRange;
public class UpdateTeamPositionRangeCmdHandler(IIdentityTeamManager<AppUser> teamMgr) : IIdCommandHandler<UpdateTeamPositionRangeCmd, TeamDto>
{    public async Task<GenResult<TeamDto>> Handle(UpdateTeamPositionRangeCmd request, CancellationToken cancellationToken)
    {
        var team = request.PrincipalTeam;
        var dto = request.Dto;

        // Create new position range
        var newPositionRange = TeamPositionRange.Create(dto.MinPosition, dto.MaxPosition);

        // Validate the position range update
        var validationResult = TeamValidators.PositionRangeUpdate.Validate(team, newPositionRange);
        if (!validationResult.Succeeded)
            return GenResult<TeamDto>.BadRequestResult(validationResult.Info);

        // Apply the validated change
        team.UpdatePositionRange(validationResult.Value!);

        var entity = await teamMgr.UpdateAsync(team);

        return GenResult<TeamDto>.Success(entity!.ToDto());
    }


}//Cls





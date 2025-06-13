using ClArch.ValueObjects;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.AppServices.Abs;
using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.Application.Features.Teams.Cmd.Create;


public class CreateCustomerTeamsCmdHandler(IIdentityTeamManager<AppUser> _teamMgr, ITeamBuilderService _teamBuilder)
    : IIdCommandHandler<CreateCustomerTeamCmd, TeamDto>
{

    public async Task<GenResult<TeamDto>> Handle(CreateCustomerTeamCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var team = _teamBuilder.CreateCustomerTeam(
            Name.Create(dto.Name),
            DescriptionNullable.Create(dto.Description));

        var entity = await _teamMgr.AddTeamAsync(team, cancellationToken);


        return GenResult<TeamDto>.Success(entity.ToDto());
    }


}//Cls


using ClArch.ValueObjects;
using ID.Domain.AppServices.Abs;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.ValueObjects;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.Options;

namespace ID.Domain.AppServices.Imps;

/// <summary>
/// Domain service implementation for building and creating Team entities.
/// Uses dependency injection to access configuration instead of static settings.
/// </summary>
internal class TeamBuilderService(
    IOptions<IdGlobalOptions> globalOptionsProvider,
    IOptions<IdGlobalSetupOptions_CUSTOMER> customerOptionsProvider) : ITeamBuilderService
{
    private readonly IdGlobalOptions _globalOptions = globalOptionsProvider.Value;
    private readonly IdGlobalSetupOptions_CUSTOMER _customerOptions = customerOptionsProvider.Value;


    //--------------------------//


    /// <inheritdoc />
    public Team CreateCustomerTeam(Name name, DescriptionNullable description)
    {
        var capacity = TeamCapacity.Create(_customerOptions.MaxTeamSize);
        var minPosition = MinTeamPosition.Create(_customerOptions.MinTeamPosition);
        var maxPosition = MaxTeamPosition.Create(_customerOptions.MaxTeamPosition);

        return Team.CreateCustomerTeam(
            name,
            description,
            capacity,
            minPosition,
            maxPosition);
    }


    //--------------------------//


    /// <inheritdoc />The _globalOptions*** Max position must be assigned to the Team's Max position, 
    public Team CreateMaintenanceTeam()
    {
        var minPosition = MinTeamPosition.Create(_globalOptions.MntcTeamMinPosition);
        var maxPosition = MaxTeamPosition.Create(_globalOptions.MntcTeamMaxPosition);
        return Team.CreateMntcTeam(minPosition, maxPosition);
    }


    //--------------------------//


    /// <inheritdoc />
    public Team CreateSuperTeam()
    {
        var minPosition = MinTeamPosition.Create(_globalOptions.SuperTeamMinPosition);
        var maxPosition = MaxTeamPosition.Create(_globalOptions.SuperTeamMaxPosition);
        return Team.CreateSuperTeam(minPosition, maxPosition);
    }


    //--------------------------//


}//Cls

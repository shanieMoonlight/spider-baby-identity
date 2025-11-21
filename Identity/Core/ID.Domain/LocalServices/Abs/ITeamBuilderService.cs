using ClArch.ValueObjects;
using ID.Domain.Entities.Teams;

namespace ID.Domain.AppServices.Abs;

/// <summary>
/// Domain service responsible for building and creating Team entities with proper configuration.
/// Encapsulates team creation logic and removes static dependencies from the Team entity.
/// </summary>
public interface ITeamBuilderService
{
    /// <summary>
    /// Creates a new customer team with the specified name and description.
    /// </summary>
    /// <param name="name">The team name</param>
    /// <param name="description">The team description (optional)</param>
    /// <returns>A new customer team with appropriate configuration</returns>
    /// <exception cref="InvalidTeamNameException">Thrown when the name conflicts with system team names</exception>
    Team CreateCustomerTeam(Name name, DescriptionNullable description);

    /// <summary>
    /// Creates the system maintenance team.
    /// </summary>
    /// <returns>A new maintenance team with system configuration</returns>
    Team CreateMaintenanceTeam();

    /// <summary>
    /// Creates the system super team.
    /// </summary>
    /// <returns>A new super team with system configuration</returns>
    Team CreateSuperTeam();
}

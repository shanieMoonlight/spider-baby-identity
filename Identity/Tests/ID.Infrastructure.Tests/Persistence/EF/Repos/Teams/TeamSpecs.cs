using ClArch.SimpleSpecification;
using ID.Domain.Entities.Teams;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.Teams;

/// <summary>
/// Specification for querying teams by team type.
/// </summary>
public class GetTeamsByTypeSpec : ASimpleSpecification<Team>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTeamsByTypeSpec"/> class.
    /// </summary>
    /// <param name="teamType">The team type to filter by.</param>
    public GetTeamsByTypeSpec(TeamType teamType)
        : base(team => team.TeamType == teamType)
    {
        SetOrderBy(query => query.OrderBy(t => t.Name));
    }

    /// <summary>
    /// Initializes a new instance for multiple team types.
    /// </summary>
    /// <param name="teamTypes">The team types to filter by.</param>
    public GetTeamsByTypeSpec(params TeamType[] teamTypes)
        : base(team => teamTypes.Contains(team.TeamType))
    {
        SetOrderBy(query => query.OrderBy(t => t.TeamType).ThenBy(t => t.Name));
    }
}

/// <summary>
/// Specification for querying teams by capacity range.
/// </summary>
public class GetTeamsByCapacityRangeSpec : ASimpleSpecification<Team>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTeamsByCapacityRangeSpec"/> class.
    /// </summary>
    /// <param name="minCapacity">Minimum capacity (null means unlimited).</param>
    /// <param name="maxCapacity">Maximum capacity (null means unlimited).</param>
    public GetTeamsByCapacityRangeSpec(int? minCapacity, int? maxCapacity)
        : base(team => 
            (minCapacity == null || team.Capacity == null || team.Capacity >= minCapacity) &&
            (maxCapacity == null || team.Capacity == null || team.Capacity <= maxCapacity))
    {
        SetOrderBy(query => query.OrderBy(t => t.Capacity ?? int.MaxValue));
    }
}

/// <summary>
/// Specification for querying teams by name contains (case-insensitive).
/// </summary>
public class GetTeamsByNameContainsSpec : ASimpleSpecification<Team>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTeamsByNameContainsSpec"/> class.
    /// </summary>
    /// <param name="nameFragment">The text that the team name should contain.</param>
    public GetTeamsByNameContainsSpec(string nameFragment)
        : base(team => team.Name.ToLower().Contains(nameFragment.ToLower()))
    {
        // Set short circuit condition to avoid query execution if name fragment is not provided
        SetShortCircuit(() => string.IsNullOrWhiteSpace(nameFragment));

        SetOrderBy(query => query.OrderBy(t => t.Name));
    }
}

/// <summary>
/// Specification for querying teams by position range.
/// </summary>
public class GetTeamsByPositionRangeSpec : ASimpleSpecification<Team>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTeamsByPositionRangeSpec"/> class.
    /// </summary>
    /// <param name="minPosition">The minimum position value.</param>
    /// <param name="maxPosition">The maximum position value.</param>
    public GetTeamsByPositionRangeSpec(int minPosition, int maxPosition)
        : base(team => 
            team.MinPosition >= minPosition && 
            team.MaxPosition <= maxPosition)
    {
        SetOrderBy(query => query.OrderBy(t => t.MinPosition).ThenBy(t => t.MaxPosition));
    }
}

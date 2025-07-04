using Microsoft.EntityFrameworkCore;
using ClArch.SimpleSpecification;
using ID.Domain.Entities.Teams;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;

/// <summary>
/// Specification for querying a team by its type and including a specific member with their address.
/// </summary>
internal class TeamTypeWithMemberSpec : ASimpleSpecification<Team>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TeamTypeWithMemberSpec"/> class.
    /// </summary>
    /// <param name="teamType">The type of the team to query.</param>
    /// <param name="userId">The ID of the user to include in the query.</param>
    /// <param name="maxPosition">The maximum position of the members to include in the query.</param>
    public TeamTypeWithMemberSpec(TeamType teamType, Guid? userId, int maxPosition = 1000)
        : base(tm => tm.TeamType == teamType)
    {
        // Set short circuit condition to avoid query execution if userId is not provided
        SetShortCircuit(() => !userId.HasValue);

        // Include the specified member and their address in the query
        SetInclude(query =>
            query.Include(e => e.Members
                    .Where(m => m.TeamPosition <= maxPosition)
                    .Where(m => m.Id == userId))
                .ThenInclude(m => m.Address)
        );
    }
}


//#################################################################//


/// <summary>
/// Specification for querying super teams with members up to a certain position.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SuperTeamWithMemberSpec"/> class.
/// </remarks>
/// <param name="userId">The ID of the user to include in the query.</param>
/// <param name="maxPosition">The maximum position of the members to include in the query.</param>
internal class SuperTeamWithMemberSpec(Guid? userId, int maxPosition = 1000)
    : TeamTypeWithMemberSpec(TeamType.super, userId, maxPosition)
{ }


//#################################################################//


/// <summary>
/// Specification for querying maintenance teams with members up to a certain position.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MntcTeamWithMemberSpec"/> class.
/// </remarks>
/// <param name="userId">The ID of the user to include in the query.</param>
/// <param name="maxPosition">The maximum position of the members to include in the query.</param>
internal class MntcTeamWithMemberSpec(Guid? userId, int maxPosition = 1000)
    : TeamTypeWithMemberSpec(TeamType.maintenance, userId, maxPosition)
{ }


//#################################################################//

//{
//    "id": "08dd359d-c901-790f-18c0-4d9594530000",
//    "firstName": "qwerty",
//    "lastName": "qwerty",
//    "phoneNumber": null,
//    "userName": "qwerty",
//    "email": "qwerty@qwerty.com",
//    "teamId": "08dd1f7e-fdd9-17f4-18c0-4d9514890000",
//    "teamPosition": 2,
//    "twoFactorProvider": "Email",
//    "twoFactorEnabled": false,
//    "emailConfirmed": false,
//    "address": null,
//    "administratorUsername": null,
//    "administratorId": null,
//    "dateCreated": "15-Jan-2025",
//    "lastModifiedDate": null
//}|
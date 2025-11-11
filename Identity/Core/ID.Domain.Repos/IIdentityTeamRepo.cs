using ID.Domain.Entities.Teams;
using ID.Domain.Repos.GenRepo;

namespace ID.Domain.Repos;

/// <summary>
/// Interface for Identity Team Repository
/// </summary>
internal interface IIdentityTeamRepo : IGenCrudRepo<Team> { }


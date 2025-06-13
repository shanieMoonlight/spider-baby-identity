using ID.Domain.Entities.Teams;
using ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;

namespace ID.Infrastructure.Persistance.Abstractions.Repos;

/// <summary>
/// Interface for Identity Team Repository
/// </summary>
internal interface IIdentityTeamRepo : IGenCrudRepo<Team> { }


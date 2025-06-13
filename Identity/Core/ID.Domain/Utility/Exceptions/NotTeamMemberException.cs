using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;

namespace ID.Domain.Utility.Exceptions;

public sealed class NotTeamMemberException<TUser>(Team team, TUser user) 
    : MyIdException($"User {user.UserName?? user.Email} ({user.Id}) is not a member of team {team.Name}") where TUser : AppUser
{ }
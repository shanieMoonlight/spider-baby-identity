using ID.Domain.Entities.Teams;

namespace ID.Domain.Utility.Exceptions;
internal class TeamCapacityExceededException(Team team) 
    : MyIdException($"The Team capacity, {team.Capacity} has been reached for team, {team.Name} has already reached") { };
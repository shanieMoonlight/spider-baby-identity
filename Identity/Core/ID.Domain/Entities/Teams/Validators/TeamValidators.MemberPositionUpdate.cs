using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Domain.Entities.Teams.Validators;

public static partial class TeamValidators
{
    public static class MemberPositionUpdate
    {
        public sealed class Token : IValidationToken
        {
            internal Token(Team team, AppUser member, TeamPosition clampedPosition)
            {
                Team = team;
                Member = member;
                ClampedPosition = clampedPosition;
            }

            public Team Team { get; }
            public AppUser Member { get; }
            public TeamPosition ClampedPosition { get; }
        }

        //------------------------//   

        public static GenResult<Token> Validate(Team team, AppUser member, TeamPosition position)
        {
            // Business rule: Member must belong to team
            if (!team.Members.Any(m => m.Id == member.Id))
                 return GenResult<Token>.BadRequestResult(IDMsgs.Error.Teams.NOT_TEAM_MEMBER(member, team));

            // Business rule: Cannot update leader position - use SetLeader method instead
            if (member.Id == team.LeaderId)
                 return GenResult<Token>.BadRequestResult(IDMsgs.Error.Teams.CANT_CHANGE_POSITION_OF_LEADER);

            // Auto-clamp position to valid team range
            var clampedPosition = TeamPosition.Create(team.EnsureValidPosition(position.Value));

            return GenResult<Token>.Success(new Token(team, member, clampedPosition));
        }

    }//Cls
}//Cls

using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Domain.Entities.Teams.Validators;

public partial class TeamValidators
{
    public sealed class MemberRemoval
    {
        public sealed class Token : IValidationToken
        {
            internal Token(Team team, AppUser member)
            {
                Team = team;
                Member = member;
            }

            public Team Team { get; }
            public AppUser Member { get; }
        }

        //-----------------------//

        public static GenResult<Token> Validate(Team team, AppUser member)
        {
            // Business rule: Member must be on the team
            if (team.Id != member.TeamId)
                return GenResult<Token>.BadRequestResult(IDMsgs.Error.Teams.NOT_TEAM_MEMBER(member, team));

            // Business rule: Cannot remove the last Super or Maintenance member,
            // Customer teams will delete the last member/leader when closing account
            if (team.TeamType != TeamType.customer && team.Members.Count == 1)
                return GenResult<Token>.BadRequestResult(IDMsgs.Error.Teams.CANT_REMOVE_LAST_MAINTENANCE_OR_SUPER_MEMBER);

            // Business rule: Removing the last Customer member is allowed
            if (team.TeamType == TeamType.customer && team.Members.Count == 1)
                return GenResult<Token>.Success(new Token(team, member));

            // Business rule: Cannot remove team leader without transferring leadership first
            if (team.LeaderId == member.Id)
                return GenResult<Token>.BadRequestResult(IDMsgs.Error.Teams.CANT_REMOVE_TEAM_LEADER);


            return GenResult<Token>.Success(new Token(team, member));
        }

    }//Cls

}//Cls

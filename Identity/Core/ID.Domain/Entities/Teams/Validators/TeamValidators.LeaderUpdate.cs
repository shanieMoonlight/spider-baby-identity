using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Domain.Entities.Teams.Validators;

public partial class TeamValidators
{
    public sealed class LeaderUpdate
    {
        public sealed class Token : IValidationToken
        {
            internal Token(Team team, AppUser member)
            {
                Team = team;
                NewLeader = member;
            }

            public Team Team { get; }
            public AppUser NewLeader { get; }
        }

        //-----------------------//

        public static GenResult<Token> Validate(Team team, AppUser newLeader)
        {
            // Business rule: Member can't belong to another team
            if (newLeader.TeamId != Guid.Empty && newLeader.TeamId != team.Id)
                 return GenResult<Token>.BadRequestResult(IDMsgs.Error.Teams.NOT_TEAM_MEMBER(newLeader, team));

            //Already the leader?
            if (newLeader.Id == team.LeaderId) 
                 return GenResult<Token>.BadRequestResult(IDMsgs.Error.Teams.ALREADY_THE_TEAM_LEADER(newLeader, team));

            return GenResult<Token>.Success(
                new Token(team, newLeader));
        }

    }//Cls

}//Cls

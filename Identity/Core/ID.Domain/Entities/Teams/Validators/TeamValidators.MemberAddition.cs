using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Domain.Entities.Teams.Validators;

public partial class TeamValidators
{

    public sealed class MemberAddition
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
            // Business rule: Check capacity
            if (team.Capacity is not null && team.Members.Count >= team.Capacity)
                 return GenResult<Token>.BadRequestResult(IDMsgs.Error.Teams.CAPACITY_EXCEEDED(team));

            // Business rule: Member can't belong to another team
            if (member.TeamId != Guid.Empty && member.TeamId != team.Id)
                 return GenResult<Token>.BadRequestResult(IDMsgs.Error.Teams.NOT_TEAM_MEMBER(member, team));

            // Business rule: Can't add same member twice
            if (team.Members.Any(m => m.Id == member.Id))
                 return GenResult<Token>.BadRequestResult(IDMsgs.Error.Teams.ALREADY_A_TEAM_MEMBER(member, team));

            return GenResult<Token>.Success(
                new Token(team, member));
        }

    }

}//Cls

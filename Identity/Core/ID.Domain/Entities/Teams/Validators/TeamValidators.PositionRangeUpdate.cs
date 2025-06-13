using ID.Domain.Entities.Teams.ValueObjects;
using MyResults;

namespace ID.Domain.Entities.Teams.Validators;

public static partial class TeamValidators
{
    public static class PositionRangeUpdate
    {
        public sealed class Token : IValidationToken
        {
            internal Token(Team team, TeamPositionRange newPositionRange)
            {
                Team = team;
                NewPositionRange = newPositionRange;
            }

            public Team Team { get; }
            public TeamPositionRange NewPositionRange { get; }
        }


        //------------------------//   

        public static GenResult<Token> Validate(Team team, TeamPositionRange newPositionRange)
        {
            // Currently no business rules - TeamPositionRange value object handles range validation
            // Member position updates handled by TeamPositionRangeUpdatedDomainEvent
            return GenResult<Token>.Success(new Token(team, newPositionRange));
        }
    
    }//Cls

}//Cls

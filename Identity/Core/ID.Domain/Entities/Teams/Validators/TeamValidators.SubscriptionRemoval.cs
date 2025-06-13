using MyResults;

namespace ID.Domain.Entities.Teams.Validators;

public static partial class TeamValidators
{
    public static class SubscriptionRemoval
    {
        /// <summary>
        /// Token representing validated parameters for removing a subscription from a team.
        /// </summary>
        public sealed class Token : IValidationToken
        {
            internal Token(Team team, TeamSubscription subscription)
            {
                Team = team;
                Subscription = subscription;
            }

            public Team Team { get; }
            public TeamSubscription Subscription { get; }
        }


        //-----------------------//

        /// <summary>
        /// Validates the removal of a subscription from a team by subscription ID.
        /// </summary>
        /// <param name="team">The team to remove the subscription from.</param>
        /// <param name="subscription">The subscription to remove.</param>
        /// <returns>A validation result containing the subscription removal token if successful.</returns>
        public static GenResult<Token> Validate(Team team, TeamSubscription subscription)
        {

            // Create and return the validation token
            var token = new Token(team, subscription);
            return GenResult<Token>.Success(token);
        }

    }//Cls

}//Cls

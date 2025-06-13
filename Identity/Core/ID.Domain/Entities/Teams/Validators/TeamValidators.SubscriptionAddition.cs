using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using MyResults;

namespace ID.Domain.Entities.Teams.Validators;

public static partial class TeamValidators
{
    public static class SubscriptionAddition
    {
        /// <summary>
        /// Token representing validated parameters for adding a subscription to a team.
        /// </summary>
        public sealed class Token : IValidationToken
        {
            internal Token(Team team, SubscriptionPlan subscriptionPlan, Discount? discount)
            {
                Team = team;
                SubscriptionPlan = subscriptionPlan;
                Discount = discount;
            }

            public Team Team { get; }
            public SubscriptionPlan SubscriptionPlan { get; }
            public Discount? Discount { get; }
        }


        //-----------------------//


        /// <summary>
        /// Validates the addition of a subscription to a team.
        /// </summary>
        /// <param name="team">The team to add the subscription to.</param>
        /// <param name="plan">The subscription plan to add.</param>
        /// <param name="discount">The discount to apply (optional).</param>
        /// <returns>A validation result containing the subscription addition token if successful.</returns>
        public static GenResult<Token> Validate(Team team, SubscriptionPlan plan, Discount? discount)
        {

            // Discount is optional, so no validation needed for null values

            // Create and return the validation token
            var token = new Token(team, plan, discount);
            return GenResult<Token>.Success(token);
        }

    }//Cls


}//Cls

using ID.Domain.Entities.Teams;
using System.ComponentModel.DataAnnotations;

namespace ID.Application.Customers.Dtos.User;
public class RegisterCustomer_NoPwdDto
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public Guid? SubscriptionPlanId { get; set; }

    public int TeamPosition { get; set; } = 1;

    //-------------------------------------//

    public string GetTeamName() =>
       "Team_" + (string.IsNullOrWhiteSpace(Username) ? Email : Username);

    //-------------------------------------//

}


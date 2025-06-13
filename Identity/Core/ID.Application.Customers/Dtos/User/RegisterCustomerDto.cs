using System.ComponentModel.DataAnnotations;

namespace ID.Application.Customers.Dtos.User;
public class RegisterCustomerDto : RegisterCustomer_NoPwdDto
{
    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;

}


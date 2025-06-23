using System.ComponentModel.DataAnnotations;

namespace ID.Application.Features.Account.Cmd.Cookies.TwoFactorVerifyCookie;

public class Verify2FactorCookieDto
{    
    [Required]
    public string Code { get; set; } = string.Empty;


}//Cls
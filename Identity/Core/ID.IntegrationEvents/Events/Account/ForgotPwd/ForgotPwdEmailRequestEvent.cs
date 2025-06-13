using ID.Domain.Entities.AppUsers;
using ID.IntegrationEvents.Abstractions;

namespace ID.IntegrationEvents.Events.Account.ForgotPwd;

public record ForgotPwdEmailRequestIntegrationEvent : AIdIntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string? Phone { get; set; }
    public string ResetToken { get; set; }
    public bool IsCustomerTeam { get; set; }

    //------------------------//

    #region MassTransitCtor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Required for MassTransit. Do not use.
    /// </summary>
    public ForgotPwdEmailRequestIntegrationEvent() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    //- - - - - - - - - - - - - - - - - - //

    public ForgotPwdEmailRequestIntegrationEvent(
        Guid userId,
        string email,
        string? phone,
        string name,
        string resetToken,
        bool isCustomerTeam)
    {
        Email = email;
        Phone = phone;
        Name = name;
        ResetToken = resetToken;
        IsCustomerTeam = isCustomerTeam;
        UserId = userId;
    }

    //- - - - - - - - - - - - - - - - - - //

    public ForgotPwdEmailRequestIntegrationEvent(AppUser user, string resetToken, bool isCustomerTeam)
    {
        Email = user.Email ?? string.Empty; //Let the consumer handle it.  
        Phone = user.PhoneNumber;
        Name = user.FirstName ?? user.UserName ?? "User";
        ResetToken = resetToken;
        IsCustomerTeam = isCustomerTeam;
        UserId = user.Id;
    }

    //------------------------//

}//Cls

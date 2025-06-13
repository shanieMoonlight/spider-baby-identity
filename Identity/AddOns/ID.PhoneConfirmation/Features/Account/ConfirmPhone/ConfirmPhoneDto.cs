namespace ID.PhoneConfirmation.Features.Account.ConfirmPhone;

//No user Id to ensure user is logged in before confirming phone
public record ConfirmPhoneDto(string ConfirmationToken);
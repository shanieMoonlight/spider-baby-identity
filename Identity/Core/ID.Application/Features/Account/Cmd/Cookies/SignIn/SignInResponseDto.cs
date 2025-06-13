using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Cookies.SignIn;
public class SignInResultDto
{
    public bool Succeeded { get; set; } = false;
    public bool EmailConfirmedRequired { get; set; } = false;
    public bool TwoFactorRequired { get; set; } = false;
    public bool NotFound { get; set; } = false;
    public bool Unauthorized { get; set; } = false;
    public string Message { get; set; } = string.Empty;

    //------------------------------------//

    public static SignInResultDto Success(string msg) =>
        new()
        {
            Succeeded = true,
            Message = msg ?? "Success!"
        };


    //- - - - - - - - - - - - - - - - - - //

    public static SignInResultDto Failure(string reason) =>
        new()
        {
            Succeeded = false,
            Message = reason
        };


    //- - - - - - - - - - - - - - - - - - //

    public static SignInResultDto NotFoundResult() =>
        new()
        {
            Succeeded = false,
            NotFound = true,
            Message = IDMsgs.Error.Authorization.INVALID_LOGIN
        };


    //- - - - - - - - - - - - - - - - - - //

    public static SignInResultDto UnauthorizedResult() =>
        new()
        {
            Succeeded = false,
            Unauthorized = true,
            Message = IDMsgs.Error.Authorization.INVALID_LOGIN
        };


    //- - - - - - - - - - - - - - - - - - //

    public static SignInResultDto EmailConfirmedRequiredResult(string email) =>
        new()
        {
            Succeeded = false,
            EmailConfirmedRequired = true,
            Message = IDMsgs.Error.Email.EMAIL_NOT_CONFIRMED(email)
        };


    //- - - - - - - - - - - - - - - - - - //

    public static SignInResultDto TwoFactorRequiredResult() =>
        new()
        {
            Succeeded = false,
            TwoFactorRequired = true,
            Message = IDMsgs.Error.Authorization.TWO_FACTOR_REQUIRED
        };

    //------------------------------------//
}

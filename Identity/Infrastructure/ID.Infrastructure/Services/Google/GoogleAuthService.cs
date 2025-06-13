using Google.Authenticator;
using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.Options;
using MyResults;


namespace ID.Infrastructure.Services.Google;
internal class GoogleAuthService(IOptions<IdGlobalOptions> _globalOptionsProvider) : IAuthenticatorAppService
{

    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;


    //-----------------------//


    public Task<AuthAppSetupDto> Setup(AppUser user)
    {
        var twoFactor = new TwoFactorAuthenticator();

        // We need a unique PER USER key to identify this Setup
        // must be saved: you need this value later to verify a validation code
        var customerSecretKey = user.Id.ToString();

        var setupInfo = twoFactor.GenerateSetupCode(
            // name of the application - the name shown in the Authenticator
            _globalOptions.ApplicationName,
            // an account identifier - shouldn't have spaces
            user.Email ?? user.UserName,
            // the secret key that also is used to validate later
            customerSecretKey.ToString(),
            // Base32 Encoding (odd this was left in)
            false,
            // resolution for the QR Code - larger number means bigger image
            4);

        AuthAppSetupDto dto = new(setupInfo.ManualEntryKey, setupInfo.QrCodeSetupImageUrl, customerSecretKey, setupInfo.Account);
        return Task.FromResult(dto);

    }

    //-----------------------//

    public Task<BasicResult> EnableAsync(AppUser user, string twoFactorCode)
    {
        var twoFactor = new TwoFactorAuthenticator();

        bool isValid = twoFactor.ValidateTwoFactorPIN(user.Id.ToString(), twoFactorCode);
        if (!isValid)
            return Task.FromResult(BasicResult.Failure(IDMsgs.Error.GoogleAuth.InvalidPin));

        return Task.FromResult(BasicResult.Success());

    }

    //-----------------------//

    public Task<bool> ValidateAsync(AppUser user, string twoFactorCode)
    {
        var twoFactor = new TwoFactorAuthenticator();

        bool isValid = twoFactor.ValidateTwoFactorPIN(user.Id.ToString(), twoFactorCode);

        return Task.FromResult(isValid);

    }

}//Cls

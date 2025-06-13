using Google.Apis.Auth;
using ID.GlobalSettings.Errors;
using ID.OAuth.Google.Auth.Abs;
using ID.OAuth.Google.Data;
using ID.OAuth.Google.Setup;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using MyResults;

namespace ID.OAuth.Google.Auth.Imps;
internal class GoogleTokenVerifier(
    Microsoft.Extensions.Options.IOptions<IdOAuthGoogleOptions> optionsProvider,
    ILogger<GoogleTokenVerifier> _logger) 
    : IGoogleTokenVerifier
{
    private readonly IdOAuthGoogleOptions _options = optionsProvider.Value;
    public async Task<GenResult<GoogleVerifiedPayload>> VerifyTokenAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                jwt: token,
                validationSettings: new GoogleJsonWebSignature.ValidationSettings { 
                    Audience = [_options.ClientId],
                });

            var myIdPayload = new GoogleVerifiedPayload(payload);

            return GenResult<GoogleVerifiedPayload>.Success(myIdPayload);

        }
        catch (InvalidJwtException ex)
        {
            _logger.LogException(ex, IdErrorEvents.OAuth.Verification);
            return GenResult<GoogleVerifiedPayload>.UnauthorizedResult(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogException(ex, IdErrorEvents.OAuth.Verification);
            return GenResult<GoogleVerifiedPayload>.UnauthorizedResult(ex.Message);
        }
    }

    //Test exceptions are logged and return UnauthorizedResult with the error message


}//Cls
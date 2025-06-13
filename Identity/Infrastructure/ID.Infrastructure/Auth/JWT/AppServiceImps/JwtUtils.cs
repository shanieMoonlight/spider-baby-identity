using ID.Application.JWT;
using ID.Domain.Models;
using ID.Domain.Utility.Exceptions;
using ID.GlobalSettings.JWT;
using ID.Infrastructure.Auth.JWT.Setup;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace ID.Infrastructure.Auth.JWT.AppServiceImps;
internal class JwtUtils(IOptions<JwtOptions> _jwtOptionsProvider) : IJwtUtils
{

    private readonly JwtOptions _jwtOptions = _jwtOptionsProvider.Value;


    //-----------------------//

    /// <summary>
    /// Turn Json into token
    /// </summary>
    /// <param name="jsonString">JSON</param>
    /// <returns>Token object</returns>
    public JwtPackage? DeserializeJwtPackage(string jsonString) =>
        JsonConvert.DeserializeObject<JwtPackage>(jsonString);


    //-----------------------//


    /// <summary>
    /// Check signature and Issuer of token
    /// </summary>
    /// <returns>Details of validation</returns>
    public JwtValidationResult Validate(JwtPackage tknPackage) => Validate(tknPackage.AccessToken);


    //-----------------------//


    /// <summary>
    /// Check signature and Issuer of token
    /// </summary>
    /// <returns>Details of validation</returns>
    public JwtValidationResult Validate(string accessToken)
    {
        JwtSecurityTokenHandler handler = new(); TokenValidationParameters tokenValidationParameters = new()
        {
            IssuerSigningKey = _jwtOptions.SecurityKey,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidIssuer = _jwtOptions.TokenIssuer
        };


        bool canRead = handler.CanReadToken(accessToken);
        if (!canRead)
            return JwtValidationResult.Failure(new CantReadTokenException());


        try
        {

            handler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken validatedToken);
            var jwtSecurityToken = validatedToken as JwtSecurityToken;

            if (jwtSecurityToken is not null)
                return JwtValidationResult.Success(jwtSecurityToken);
            else
                return JwtValidationResult.Failure(null);

        }
        catch (SecurityTokenValidationException validationException)
        {
            return JwtValidationResult.Failure(validationException);
        }

    }



}//Cls

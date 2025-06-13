using ID.Domain.Models;
using ID.GlobalSettings.JWT;

namespace ID.Application.JWT;
public interface IJwtUtils
{
    JwtPackage? DeserializeJwtPackage(string jsonString);
    JwtValidationResult Validate(JwtPackage tknPackage);
    JwtValidationResult Validate(string accessToken);
}
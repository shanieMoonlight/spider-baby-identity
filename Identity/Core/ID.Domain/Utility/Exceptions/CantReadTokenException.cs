using Microsoft.IdentityModel.Tokens;

namespace ID.Domain.Utility.Exceptions;

/// <summary>
/// Exception to let user know that there was some problem reading the JWT. 
/// </summary>
public class CantReadTokenException(string message = "") : SecurityTokenValidationException(message ?? "")
{ }
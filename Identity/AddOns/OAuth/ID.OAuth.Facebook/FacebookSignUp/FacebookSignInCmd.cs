using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Models;

namespace ID.OAuth.Facebook.FacebookSignUp;

/// <summary>
/// Command for Facebook OAuth sign-in operations.
/// </summary>
public record FacebookSignInCmd(FacebookSignInDto Dto) : AIdCommand<JwtPackage>;

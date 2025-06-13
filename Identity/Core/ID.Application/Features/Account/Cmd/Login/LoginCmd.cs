using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Models;

namespace ID.Application.Features.Account.Cmd.Login;

/// <summary>
/// Command to authenticate a user and generate JWT tokens for application access.
/// Supports login via username, email, or user ID with password authentication.
/// </summary>
/// <param name="Dto">The login data transfer object containing user credentials and login options</param>
public record LoginCmd(LoginDto Dto) : AIdCommand<JwtPackage>;




using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Models;

namespace ID.OAuth.Google.GoogleSignUp;
public record GoogleSignInCmd(GoogleSignInDto Dto) : AIdCommand<JwtPackage>;




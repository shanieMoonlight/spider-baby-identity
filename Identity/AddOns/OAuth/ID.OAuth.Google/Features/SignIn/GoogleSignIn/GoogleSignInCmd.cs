using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Models;

namespace ID.OAuth.Google.Features.SignIn.GoogleSignIn;
public record GoogleSignInCmd(GoogleSignInDto Dto) : AIdCommand<JwtPackage>;




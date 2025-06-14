using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Models;

namespace ID.OAuth.Google.Features.GoogleSignIn;
public record GoogleSignInCmd(GoogleSignInDto Dto) : AIdCommand<JwtPackage>;




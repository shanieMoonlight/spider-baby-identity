using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Models;

namespace ID.OAuth.Facebook.Features.SignIn.FacebookSignIn;
public record FacebookSignInCmd(FacebookSignInDto Dto) : AIdCommand<JwtPackage>;




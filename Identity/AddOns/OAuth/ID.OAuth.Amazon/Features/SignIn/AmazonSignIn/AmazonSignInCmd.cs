using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Models;

namespace ID.OAuth.Amazon.Features.SignIn.AmazonSignIn;

public record AmazonSignInCmd(AmazonSignInDto Dto) : AIdCommand<JwtPackage>;

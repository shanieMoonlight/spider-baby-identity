using ID.Domain.Entities.AppUsers;
using ID.OAuth.Google.Data;
using ID.OAuth.Google.Features.SignIn;
using MyResults;

namespace ID.OAuth.Google.Services.Abs;
public interface IFindOrCreateService<TUser> where TUser : AppUser
{
    Task<GenResult<AppUser>> FindOrCreateUserAsync(GoogleVerifiedPayload payload, GoogleSignInDto dto, CancellationToken cancellationToken);
}
using ID.Domain.Entities.AppUsers;
using ID.OAuth.Facebook.Data;
using ID.OAuth.Facebook.Features.SignIn;
using MyResults;

namespace ID.OAuth.Facebook.Services;
public interface IFindOrCreateService<TUser> where TUser : AppUser
{
    Task<GenResult<AppUser>> FindOrCreateUserAsync(FacebookUserProfile userProfile, FacebookSignInDto dto, CancellationToken cancellationToken);
}
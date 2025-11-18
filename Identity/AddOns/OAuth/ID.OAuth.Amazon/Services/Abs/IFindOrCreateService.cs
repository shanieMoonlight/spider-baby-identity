using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.OAuth.Amazon.Services.Abs;

public interface IFindOrCreateService<TUser> where TUser : AppUser
{
    Task<GenResult<AppUser>> FindOrCreateUserAsync(
        ID.OAuth.Amazon.Data.AmazonUserProfile userProfile,
        ID.OAuth.Amazon.Features.SignIn.AmazonSignInDto dto,
        CancellationToken cancellationToken);
}

using ID.Domain.Entities.AppUsers;

namespace ID.Application.AppAbs.ApplicationServices.User;
public interface IFindUserService<TUser> where TUser : AppUser
{
    Task<TUser?> FindUserWithTeamDetailsAsync(string? email = null, string? username = null, Guid? userId = null);
    Task<TUser?> FindUserWithTeamDetailsAsync(Guid? userId);
    Task<TUser?> FindUserAsync(Guid? userId);
    Task<TUser?> FindUserByEmailAsync(string? email);
    Task<TUser?> FindUserByUsernameAsync(string? username);
}
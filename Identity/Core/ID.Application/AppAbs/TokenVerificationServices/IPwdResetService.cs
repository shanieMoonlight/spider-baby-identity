using MyResults;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.AppAbs.TokenVerificationServices;
public interface IPwdResetService<TUser> where TUser : AppUser
{
    Task<BasicResult> ChangePasswordAsync(Team team, TUser user, string password);
    Task<string> GeneratePasswordResetTokenAsync(Team team, TUser user);
    Task<string> GenerateSafePasswordResetTokenAsync(Team team, TUser user);
    Task<BasicResult> ResetPasswordAsync(Team team, TUser user, string token, string newPassword, CancellationToken cancellationToken = default);
}
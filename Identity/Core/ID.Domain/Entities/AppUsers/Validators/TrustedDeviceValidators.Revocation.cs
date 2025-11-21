using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Domain.Entities.AppUsers.Validators;

public static partial class TrustedDeviceValidators
{
    public sealed class Revocation
    {
        public sealed class Token : IUserValidationToken
        {
            internal Token(AppUser user, TrustedDevice device)
            {
                User = user;
                Device = device;
            }

            public AppUser User { get; }
            public TrustedDevice Device { get; }
        }

        //-----------------------//

        public static GenResult<Token> Validate(AppUser user, TrustedDevice device)
        {
            if (device.UserId != user.Id)
                return GenResult<Token>.BadRequestResult(IDMsgs.Error.TrustedDevices.USER_NOT_OWNER(device, user));


            return GenResult<Token>.Success(new Token(user, device));
        }
    }

}

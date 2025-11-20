using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Domain.Entities.AppUsers.Validators;

public static partial class TrustedDeviceValidators
{
    public sealed class Addition
    {
        public sealed class Token : IUserValidationToken
        {
            internal Token(AppUser user, DeviceFingerprint deviceFingerprint, DeviceName deviceName, UserAgent userAgent, TrustedUntil trustedUntil)
            {
                User = user;
                DeviceFingerprint = deviceFingerprint;
                DeviceName = deviceName;
                UserAgent = userAgent;
                TrustedUntil = trustedUntil;
            }

            public AppUser User { get; }
            public DeviceFingerprint DeviceFingerprint { get; }
            public DeviceName DeviceName { get; }
            public UserAgent UserAgent { get; }
            public TrustedUntil TrustedUntil { get; }
        }

        //-----------------------//

        public static GenResult<Token> Validate(AppUser user, DeviceFingerprint deviceFingerprint, DeviceName deviceName, UserAgent userAgent, TrustedUntil trustedUntil)
        {
            // Business rule: Max 10 trusted devices per user
            const int MAX = 10;
            if (user.TrustedDevices.Count >= MAX)
                return GenResult<Token>.BadRequestResult(IDMsgs.Error.TrustedDevices.MAX_EXCEEDED(user, MAX));

            // Business rule: Device fingerprint not already trusted by this user (and not active)
            var existing = user.TrustedDevices.FirstOrDefault(d => d.DeviceFingerprint == deviceFingerprint.Value && !d.IsExpired());
            if (existing is not null)
                return GenResult<Token>.BadRequestResult(IDMsgs.Error.TrustedDevices.ALREADY_TRUSTED(existing, user));

            return GenResult<Token>.Success(new Token(user, deviceFingerprint, deviceName, userAgent, trustedUntil));
        }
    }

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
            //if (device is null)
            //    return GenResult<Token>.BadRequestResult(IDMsgs.Error.TrustedDevices.NOT_FOUND(device, user));

            if (device.UserId != user.Id)
                return GenResult<Token>.BadRequestResult(IDMsgs.Error.TrustedDevices.NOT_OWNED(device, user));

            if (device.IsExpired())
                return GenResult<Token>.BadRequestResult(IDMsgs.Error.TrustedDevices.ALREADY_REVOKED(device, user));

            return GenResult<Token>.Success(new Token(user, device));
        }
    }

}

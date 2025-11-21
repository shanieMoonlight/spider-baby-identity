using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Domain.Entities.AppUsers.Validators;

public  partial class TrustedDeviceValidators
{
    public sealed class Addition
    {
        public sealed class Token : IUserValidationToken
        {
            internal Token(
                AppUser user, 
                DeviceFingerprint deviceFingerprint, 
                DeviceName deviceName, 
                UserAgent userAgent, 
                TrustDurationNullable trustDuration)
            {
                User = user;
                DeviceFingerprint = deviceFingerprint;
                DeviceName = deviceName;
                UserAgent = userAgent;
                TrustDuration = trustDuration;
            }

            public AppUser User { get; }
            public DeviceFingerprint DeviceFingerprint { get; }
            public DeviceName DeviceName { get; }
            public UserAgent UserAgent { get; }
            public TrustDurationNullable TrustDuration { get; }
        }

        //-----------------------//

        public static GenResult<Token> Validate(
            AppUser user, 
            DeviceFingerprint deviceFingerprint, 
            DeviceName deviceName, 
            UserAgent userAgent, 
            TrustDurationNullable trustDuration)
        {
            // Business rule: Max 10 trusted devices per user
            const int MAX = 10;
            if (user.TrustedDevices.Count >= MAX)
                return GenResult<Token>.BadRequestResult(IDMsgs.Error.TrustedDevices.MAX_EXCEEDED(user, MAX));

            // Business rule: Device fingerprint not already trusted by this user (and not active)
            var existing = user.TrustedDevices.FirstOrDefault(d => d.DeviceFingerprint == deviceFingerprint.Value && !d.IsExpired());
            if (existing is not null)
                return GenResult<Token>.BadRequestResult(IDMsgs.Error.TrustedDevices.ALREADY_TRUSTED(existing, user));

            return GenResult<Token>.Success(new Token(user, deviceFingerprint, deviceName, userAgent, trustDuration));
        }
    }

}

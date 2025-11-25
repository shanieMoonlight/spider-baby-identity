using ID.Domain.Entities.TrustedDevices.ValueObjects;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Setup.Defaults;
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
                IpAddress ipAddress,
                TrustDuration trustDuration)
            {
                User = user;
                DeviceFingerprint = deviceFingerprint;
                DeviceName = deviceName;
                UserAgent = userAgent;
                TrustDuration = trustDuration;
                IpAddress = ipAddress;
            }

            public AppUser User { get; }
            public DeviceFingerprint DeviceFingerprint { get; }
            public DeviceName DeviceName { get; }
            public UserAgent UserAgent { get; }
            public TrustDuration TrustDuration { get; }
            public IpAddress IpAddress { get; }
        }

        //-----------------------//

        public static GenResult<Token> Validate(
            AppUser user, 
            DeviceFingerprint deviceFingerprint, 
            DeviceName deviceName, 
            UserAgent userAgent,
            IpAddress ipAddress, 
            TrustDuration trustDuration)
        {
            // Business rule: Limit trusted devices per user
            const int MAX = IdGlobalDefaultValues.MAX_TRUSTED_DEVICES_PER_USER;
            if (user.TrustedDevices.Count >= MAX)
                return GenResult<Token>.BadRequestResult(IDMsgs.Error.TrustedDevices.MAX_EXCEEDED(user, MAX));

            // Business rule: Device fingerprint not already trusted by this user (and not active)
            // If Expired adding device will extend trust, so we let it pass
            var existing = user.TrustedDevices.FirstOrDefault(d => d.Fingerprint == deviceFingerprint.Value && !d.IsExpired());
            if (existing is not null)
                return GenResult<Token>.BadRequestResult(IDMsgs.Error.TrustedDevices.ALREADY_TRUSTED(existing, user));

            return GenResult<Token>.Success(new Token(user, deviceFingerprint, deviceName, userAgent, ipAddress, trustDuration));
        }
    }

}

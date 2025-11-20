using ClArch.ValueObjects.Common;

namespace ID.Domain.Entities.TrustedDevices.ValueObjects;

//=============================================================================//

public sealed class TrustedUntil : ValueObject<DateTime?>
{
    private TrustedUntil(DateTime? value) : base(value) { }

    public static TrustedUntil Create(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new ArgumentException("TrustedUntil date must be in UTC.", nameof(value));

        if (value <= DateTime.UtcNow)
            throw new ArgumentException("TrustedUntil date must be in the future.", nameof(value));

        return new TrustedUntil(value);
    }

    public static TrustedUntil CreateNullable(DateTime? value)
    {
        if (value == null)
            return new TrustedUntil(null);

        if (value.Value.Kind != DateTimeKind.Utc)
            throw new ArgumentException("TrustedUntil date must be in UTC.", nameof(value));

        if (value.Value <= DateTime.UtcNow)
            throw new ArgumentException("TrustedUntil date must be in the future.", nameof(value));

        return new TrustedUntil(value.Value);
    }

    public bool IsExpired() => Value.HasValue && Value.Value <= DateTime.UtcNow;

}

//=============================================================================//

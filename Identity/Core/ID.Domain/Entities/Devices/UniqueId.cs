using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Utility;

namespace ID.Domain.Entities.Devices;
public class UniqueId : StringValueObject
{
    public const int MaxLength = 256;

    private UniqueId(string value) : base(value) { }

    public static UniqueId Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(UniqueId));
        Ensure.MaxLength(value, MaxLength, nameof(UniqueId));

        return new(value);
    }

}//Cls





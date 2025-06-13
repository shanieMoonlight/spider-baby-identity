using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Utility;

namespace ID.Domain.Entities.AppUsers.ValueObjects;


public class IssuerNullable : NullableStringValueObject
{
    public const int MaxLength = 50;

    private IssuerNullable(string? value) : base(value) { }

    public static IssuerNullable Create(string? value)
    {
        Ensure.MaxLength(value, MaxLength, nameof(IssuerNullable));

        return new(value);
    }

}//Cls



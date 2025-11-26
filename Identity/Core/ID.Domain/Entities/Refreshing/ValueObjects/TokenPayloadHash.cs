using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Utility;

namespace ID.Domain.Entities.Refreshing.ValueObjects;


public class TokenPayloadHash : StringValueObject
{
    public const int MaxLength = 512;

    private TokenPayloadHash(string value) : base(value) { }

    public static TokenPayloadHash Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(TokenPayloadHash));
        Ensure.MaxLength(value, MaxLength, nameof(TokenPayloadHash));

        return new(value);
    }

}//Cls



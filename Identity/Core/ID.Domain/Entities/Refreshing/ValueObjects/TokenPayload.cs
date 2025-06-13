using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Utility;

namespace ID.Domain.Entities.Refreshing.ValueObjects;


public class TokenPayload : StringValueObject
{
    public const int MaxLength = 200;

    private TokenPayload(string value) : base(value) { }

    public static TokenPayload Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(TokenPayload));
        Ensure.MaxLength(value, MaxLength, nameof(TokenPayload));

        return new(value);
    }

}//Cls



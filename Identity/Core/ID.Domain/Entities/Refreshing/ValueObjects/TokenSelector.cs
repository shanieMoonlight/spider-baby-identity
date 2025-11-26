using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Utility;

namespace ID.Domain.Entities.Refreshing.ValueObjects;


public class TokenSelector : StringValueObject
{
    public const int MaxLength = 200;

    private TokenSelector(string value) : base(value) { }

    public static TokenSelector Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(TokenSelector));
        Ensure.MaxLength(value, MaxLength, nameof(TokenSelector));

        return new(value);
    }

}//Cls



using ClArch.ValueObjects.Common;
using ID.GlobalSettings.Setup.Defaults;

namespace ID.Domain.Entities.Refreshing.ValueObjects;


public class TokenLifetime : ValueObject<TimeSpan>
{

    private TokenLifetime(TimeSpan value) : base(value) { }

    public static TokenLifetime Create(TimeSpan value)
    {
        if (value <= TimeSpan.Zero)
            value = IdGlobalDefaultValues.REFRESH_TOKEN_EXPIRE_TIME_SPAN;

        return new(value);
    }

}//Cls



using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Exceptions;

namespace ID.Domain.Entities.AppUsers.ValueObjects;

//====================================================//

public class TeamPosition : ValueObject<int>
{
    private TeamPosition(int value) : base(value) { }

    public static TeamPosition Create(int value)
    {
        return new(value);
    }

}//Cls

//====================================================//

public class TeamPositionNullable : ValueObject<int?>
{
    private TeamPositionNullable(int? value) : base(value) { }

    /// <summary>
    /// Will default to <see cref="IdDomainSettings.DefaultMaxTeamPosition"/> if <paramref name="value"/> is null
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="InvalidPropertyException"></exception>
    public static TeamPositionNullable Create(int? value = null)
    {
        return new(value);
    }

}//Cls

//====================================================//
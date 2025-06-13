using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Exceptions;


namespace ID.Domain.Entities.Teams.ValueObjects;


public class MinTeamPosition : ValueObject<int>
{
    private MinTeamPosition(int value) : base(value) { }

    public static MinTeamPosition Create(int value)
    {
        if (value < 1)
            throw new InvalidPropertyException(nameof(MinTeamPosition), $"The {nameof(MinTeamPosition)}: {value} is not valid.");

        return new(value);
    }

}//Cls
using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Exceptions;


namespace ID.Domain.Entities.Teams.ValueObjects;


public class MaxTeamPosition : ValueObject<int>
{
    private MaxTeamPosition(int value) : base(value) { }

    public static MaxTeamPosition Create(int value)
    {
        if (value < 1)
            throw new InvalidPropertyException(nameof(MaxTeamPosition), $"The {nameof(MaxTeamPosition)}: {value} is not valid.");

        return new(value);
    }

}//Cls

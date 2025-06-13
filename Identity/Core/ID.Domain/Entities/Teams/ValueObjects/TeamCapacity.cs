using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Exceptions;


namespace ID.Domain.Entities.Teams.ValueObjects;


public class TeamCapacity : ValueObject<int>
{
    private TeamCapacity(int value) : base(value) { }

    public static TeamCapacity Create(int value)
    {
        if (value < 1)
            throw new InvalidPropertyException(nameof(TeamCapacity), $"The {nameof(TeamCapacity)}: {value} is not valid.");

        return new(value);
    }

}//Cls

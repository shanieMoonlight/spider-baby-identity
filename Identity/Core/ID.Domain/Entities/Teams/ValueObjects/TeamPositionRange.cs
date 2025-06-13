using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Exceptions;

namespace ID.Domain.Entities.Teams.ValueObjects;

//====================================================//

public record TeamPositionRangeData(int Min, int Max);


//====================================================//


public class TeamPositionRange : ValueObject<TeamPositionRangeData>
{
    private TeamPositionRange(TeamPositionRangeData value) : base(value) { }

    public static TeamPositionRange Create(int minPosition, int maxPosition)
    {

        if(minPosition > maxPosition)
            throw new InvalidPropertyException(nameof(TeamPositionRange), $"The {nameof(minPosition)} cannot be higher than the {nameof(maxPosition)}.");

        var data = new TeamPositionRangeData(minPosition, maxPosition);
        return new(data);
    }

}//Cls
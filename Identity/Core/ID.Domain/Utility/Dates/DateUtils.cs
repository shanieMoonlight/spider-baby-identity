namespace ID.Domain.Utility.Dates;
public static class DateUtilExtensions
{

    public static DateTime ConvertFromUnixTimestamp(this double timestamp)
    {
        DateTime origin = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return origin.AddSeconds(timestamp);
    }

    //- - - - - - - - - - - - - - - -//
    public static DateTime ConvertFromUnixTimestamp(this long timestamp)
    {
        DateTime origin = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return origin.AddSeconds(timestamp);
    }

    //-------------------------------//

    public static long ConvertToUnixTimestamp(this DateTime date)
    {

        DateTime origin = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return (long)Math.Floor(diff.TotalSeconds);

    }

    //- - - - - - - - - - - - - - - -//

    public static long ConvertToUnixTimestamp(this DateTime? date)
    {
        if (!date.HasValue)
            return 0;

        var dateValue = date.Value;

        var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = dateValue.ToUniversalTime() - origin;
        return (long)Math.Floor(diff.TotalSeconds);

    }

}

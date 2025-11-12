using System;
using ID.Application.Jobs.Abstractions;

namespace ID.Jobs.Quartz;

/// <summary>
/// Produces Quartz-compatible cron expressions.
/// </summary>
internal class QuartzCronBuilder : ICronBuilder
{

    // Every minute
    public string Minutely() => "0 0/1 * ? * *";

    // Every hour at minute 0
    public string Hourly() => "0 0 * ? * *";

    // Every hour at specified minute
    public string Hourly(int minute)
    {
        if (minute < 0 || minute > 59) throw new ArgumentOutOfRangeException(nameof(minute));
        return $"0 {minute} * ? * *";
    }

    public string Daily() => "0 0 0 ? * *";

    public string Weekly() => "0 0 0 ? * MON"; // Monday

    public string Weekly(DayOfWeek dayOfWeek) => $"0 0 0 ? * {DayOfWeekToQuartz(dayOfWeek)}";

    public string Weekly(DayOfWeek dayOfWeek, int hour)
    {
        if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException(nameof(hour));
        return $"0 0 {hour} ? * {DayOfWeekToQuartz(dayOfWeek)}";
    }

    public string Monthly() => "0 0 0 1 * ?"; // first day of month

    public string Monthly(int day)
    {
        if (day < 1 || day > 31) throw new ArgumentOutOfRangeException(nameof(day));
        return $"0 0 0 {day} * ?";
    }

    public string Monthly(int day, int hour)
    {
        if (day < 1 || day > 31) throw new ArgumentOutOfRangeException(nameof(day));
        if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException(nameof(hour));
        return $"0 0 {hour} {day} * ?";
    }

    public string Yearly() => "0 0 0 1 1 ?";

    public string Yearly(int month) 
    {
        if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));
        return $"0 0 0 1 {month} ?";
    }

    public string Yearly(int month, int day)
    {
        if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));
        if (day < 1 || day > 31) throw new ArgumentOutOfRangeException(nameof(day));
        return $"0 0 0 {day} {month} ?";
    }

    public string Yearly(int month, int day, int hour)
    {
        if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));
        if (day < 1 || day > 31) throw new ArgumentOutOfRangeException(nameof(day));
        if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException(nameof(hour));
        return $"0 0 {hour} {day} {month} ?";
    }

    public string Never() => "0 0 0 31 2 ?"; // Feb 31st (never occurs)

    public string MinuteInterval(int minutes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(minutes);
        return $"0 0/{minutes} * ? * *";
    }

    public string Expression(string cronExpression) => Normalize(cronExpression);

    //-----------------------//

    private static string DayOfWeekToQuartz(DayOfWeek dayOfWeek) => dayOfWeek switch
    {
        DayOfWeek.Sunday => "SUN",
        DayOfWeek.Monday => "MON",
        DayOfWeek.Tuesday => "TUE",
        DayOfWeek.Wednesday => "WED",
        DayOfWeek.Thursday => "THU",
        DayOfWeek.Friday => "FRI",
        DayOfWeek.Saturday => "SAT",
        _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek))
    };

    //- - - - - - - - - - - -//

    private static string Normalize(string expr)
    {
        if (string.IsNullOrWhiteSpace(expr))
            return expr ?? string.Empty;

        var parts = expr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        // If it's the standard 5-field cron (minute hour day month day-of-week),
        // prefix seconds as '0'. If already 6 or 7 fields, assume Quartz-compatible.
        if (parts.Length == 5)
            parts = ["0", parts[0], parts[1], parts[2], parts[3], parts[4]];

        // Now parts represent Quartz fields: 0=sec,1=min,2=hour,3=day-of-month,4=month,5=day-of-week,[6=year]
        // If both day-of-month and day-of-week are specified (neither is '*' or '?'), Quartz throws.
        // To convert safely, prefer day-of-month and set day-of-week to '?'.
        if (parts.Length >= 6)
        {
            var dom = parts[3];
            var dow = parts[5];

            bool domIsWildcard = string.IsNullOrEmpty(dom) || dom == "*" || dom == "?";
            bool dowIsWildcard = string.IsNullOrEmpty(dow) || dow == "*" || dow == "?";

            if (!domIsWildcard && !dowIsWildcard)
                // Prefer day-of-month semantics; set day-of-week to '?'
                parts[5] = "?";
        }

        return string.Join(' ', parts);
    }


}//Cls

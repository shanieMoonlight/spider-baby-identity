using System;

namespace ID.Application.Jobs.Abstractions;

/// <summary>
/// Abstraction for producing cron schedule expressions so the application
/// layer does not need a direct dependency on Hangfire.
/// </summary>
public interface ICronBuilder
{
 /// <summary>
 /// Returns cron expression that fires every minute.
 /// </summary>
 string Minutely();

 /// <summary>
 /// Returns cron expression that fires every hour at the first minute.
 /// </summary>
 string Hourly();

 /// <summary>
 /// Returns cron expression that fires every hour at the specified minute.
 /// </summary>
 /// <param name="minute">The minute in which the schedule will be activated (0-59).</param>
 string Hourly(int minute);

 /// <summary>
 /// Returns cron expression that fires every day at00:00 UTC.
 /// </summary>
 string Daily();

 /// <summary>
 /// Returns cron expression that fires every week at Monday,00:00 UTC.
 /// </summary>
 string Weekly();

 /// <summary>
 /// Returns cron expression that fires every week at00:00 UTC of the specified day
 /// of the week.
 /// </summary>
 /// <param name="dayOfWeek">The day of week in which the schedule will be activated.</param>
 string Weekly(DayOfWeek dayOfWeek);

 /// <summary>
 /// Returns cron expression that fires every week at the first minute of the specified
 /// day of week and hour in UTC.
 /// </summary>
 /// <param name="dayOfWeek">The day of week in which the schedule will be activated.</param>
 /// <param name="hour">The hour in which the schedule will be activated (0-23).</param>
 string Weekly(DayOfWeek dayOfWeek, int hour);

 /// <summary>
 /// Returns cron expression that fires every month at00:00 UTC of the first day
 /// of month.
 /// </summary>
 string Monthly();

 /// <summary>
 /// Returns cron expression that fires every month at00:00 UTC of the specified
 /// day of month.
 /// </summary>
 /// <param name="day">The day of month in which the schedule will be activated (1-31).</param>
 string Monthly(int day);

 /// <summary>
 /// Returns cron expression that fires every month at the first minute of the specified
 /// day of month and hour in UTC.
 /// </summary>
 /// <param name="day">The day of month in which the schedule will be activated (1-31).</param>
 /// <param name="hour">The hour in which the schedule will be activated (0-23).</param>
 string Monthly(int day, int hour);

 /// <summary>
 /// Returns cron expression that fires every year on Jan,1st at00:00 UTC.
 /// </summary>
 string Yearly();

 /// <summary>
 /// Returns cron expression that fires every year in the first day at00:00 UTC of
 /// the specified month.
 /// </summary>
 /// <param name="month">The month in which the schedule will be activated (1-12).</param>
 string Yearly(int month, int day);

 /// <summary>
 /// Returns cron expression that fires every year at00:00 UTC of the specified month
 /// and day of month.
 /// </summary>
 /// <param name="month">The month in which the schedule will be activated (1-12).</param>
 /// <param name="day">The day of month in which the schedule will be activated (1-31).</param>
 string Yearly(int month);

 /// <summary>
 /// Returns cron expression that never fires. Specifically31st of February
 /// </summary>
 string Never();

 /// <summary>
 /// Returns cron expression that fires every &lt;interval&gt; minutes.
 /// </summary>
 /// <param name="minutes">The number of minutes to wait between every activation.</param>
 string MinuteInterval(int minutes);

 /// <summary>
 /// Return the raw cron expression supplied.
 /// </summary>
 /// <param name="cronExpression">Cron expression to use as-is.</param>
 string Expression(string cronExpression);
    string Yearly(int month, int day, int hour);
}

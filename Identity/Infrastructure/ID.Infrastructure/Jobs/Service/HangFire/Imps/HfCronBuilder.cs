using Hangfire;
using ID.Application.Jobs.Abstractions;

namespace ID.Infrastructure.Jobs.Service.HangFire.Imps;

internal class HfCronBuilder : ICronBuilder
{
    public string Daily() => Cron.Daily();

    public string Weekly() => Cron.Weekly();

    public string Weekly(DayOfWeek dayOfWeek) => Cron.Weekly(dayOfWeek);

    public string Weekly(DayOfWeek dayOfWeek, int hour) => Cron.Weekly(dayOfWeek, hour);

    public string Hourly() => Cron.Hourly();

    public string Hourly(int minute) => Cron.Hourly(minute);

    public string Minutely() => Cron.Minutely();

    public string MinuteInterval(int minutes) => $"*/{minutes} * * * *";

    public string Monthly() => Cron.Monthly();

    public string Monthly(int day) => Cron.Monthly(day);

    public string Monthly(int day, int hour) => Cron.Monthly(day, hour);

    public string Yearly() => Cron.Yearly();

    public string Yearly(int month) => Cron.Yearly(month);

    public string Yearly(int month, int day) => Cron.Yearly(month, day);

    public string Yearly(int month, int day, int hour) => Cron.Yearly(month, day, hour);

    public string Never() => Cron.Never();

    public string Expression(string cronExpression) => cronExpression;

}

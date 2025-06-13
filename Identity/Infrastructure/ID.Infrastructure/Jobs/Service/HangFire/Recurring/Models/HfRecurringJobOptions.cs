using Hangfire;

namespace ID.Infrastructure.Jobs.Service.HangFire.Recurring.Models;

/// <summary>
/// Configuration options for Hangfire recurring jobs.
/// Provides settings to control timezone and misfire handling behavior.
/// </summary>
internal class HfRecurringJobOptions
{
    private TimeZoneInfo _timeZone = TimeZoneInfo.Utc;
    
    /// <summary>
    /// Gets or sets the timezone for scheduling recurring jobs.
    /// This determines how CRON expressions are interpreted.
    /// </summary>
    /// <remarks>
    /// Default value is UTC timezone. If null is provided, UTC will be used instead.
    /// </remarks>
    public TimeZoneInfo TimeZone
    {
        get => _timeZone;
        set => _timeZone = value ?? TimeZoneInfo.Utc;
    }

    private MisfireHandlingMode _misfireHandling = MisfireHandlingMode.Relaxed;
    
    /// <summary>
    /// Gets or sets how missed executions should be handled.
    /// </summary>
    /// <remarks>
    /// Default value is Relaxed mode, which means jobs will be executed
    /// only once when the schedule is missed. Strict mode will trigger
    /// multiple executions to catch up on all missed schedules.
    /// </remarks>
    public MisfireHandlingMode MisfireHandling
    {
        get => _misfireHandling;
        set => _misfireHandling = value;
    }
}


//######################################//


internal static class HfRecurringJobOptionsExtensions
{
    public static RecurringJobOptions ToRecurringJobOptions(this HfRecurringJobOptions? options)
    {
        if (options == null)
            return new();

        return new()
        {
            TimeZone = options.TimeZone,
            MisfireHandling = options.MisfireHandling
        };
    }
}



//######################################//
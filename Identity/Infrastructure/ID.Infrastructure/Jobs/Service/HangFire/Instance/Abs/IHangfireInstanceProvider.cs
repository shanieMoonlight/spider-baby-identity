using Hangfire;

namespace ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;

/// <summary>
/// Provides centralized access to Hangfire infrastructure components with isolation from the main application.
/// </summary>
/// <remarks>
/// This interface serves as the primary integration point for Hangfire operations within the MyId library.
/// It encapsulates access to storage, background jobs, and recurring jobs through a single dependency,
/// allowing the Identity library to maintain its own isolated Hangfire infrastructure (separate database schema,
/// dashboard, and processing).
/// 
/// The implementation of this interface is typically consumed by various job managers and never
/// directly by application code. This creates a clean separation between the job infrastructure
/// concerns and business logic.
/// </remarks>
internal interface IHangfireInstanceProvider
{
    /// <summary>
    /// Gets the Hangfire job storage instance used for persistence.
    /// </summary>
    /// <remarks>
    /// This storage instance is configured to use a dedicated schema/database separate from
    /// any Hangfire instance the host application might be using.
    /// </remarks>
    JobStorage Storage { get; }



    /// <summary>
    /// Gets the wrapper for Hangfire's recurring job manager functionality.
    /// </summary>
    /// <remarks>
    /// Provides operations for creating, updating, triggering, and removing recurring jobs
    /// that execute on a schedule defined by CRON expressions. The wrapper simplifies testing
    /// by abstracting Hangfire's static APIs behind an interface.
    /// </remarks>
    IRecurringJobManagerWrapper RecurringJobManager { get; }



    /// <summary>
    /// Gets the wrapper for Hangfire's background job client functionality.
    /// </summary>
    /// <remarks>
    /// Provides operations for fire-and-forget jobs, delayed jobs, and job state manipulation.
    /// The wrapper makes Hangfire's extension methods testable by implementing them behind an interface.
    /// </remarks>
    IBackgroundJobClientWrapper BackgroundJobClient { get; }


}//Cls
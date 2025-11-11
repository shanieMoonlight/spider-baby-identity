namespace ID.Application.Jobs.@Abstractions;

/// <summary>
/// Marker attribute describing a desired distributed lock for a job method or job class.
/// Application layer code can reference this attribute without depending on Hangfire.
/// The infrastructure Hangfire filter will detect this attribute and acquire/release the lock.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public sealed class MyIdDisableConcurrentExecutionAttribute : Attribute
{
    /// <summary>
    /// Optional resource template (string.Format style). If null/empty, a default resource based
    /// on job type + method will be used.
    /// </summary>
    public string? Resource { get; }

    /// <summary>
    /// Timeout in seconds for the distributed lock.
    /// </summary>
    public int TimeoutSec { get; }

    public MyIdDisableConcurrentExecutionAttribute(int timeoutInSeconds)
    {
        if (timeoutInSeconds < 0)
            throw new ArgumentException("Timeout argument value should be greater that zero.", nameof(timeoutInSeconds));

        TimeoutSec = timeoutInSeconds;
    }

    public MyIdDisableConcurrentExecutionAttribute(string resource, int timeoutSec)
    : this(timeoutSec)
    {
        Resource = resource;
    }

}//Cls

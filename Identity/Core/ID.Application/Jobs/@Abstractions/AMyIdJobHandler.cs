using System.Threading;

namespace ID.Application.Jobs.Abstractions;

/// <summary>
/// All Jobs need a JobId. 
/// This is a helper class
/// </summary>
public abstract class AMyIdJobHandler(string jobId)
{
    public string JobId { get; set; } = $"MY_ID_{jobId}";

    /// <summary>
    /// Primary handler method. Implementations may override the CancellationToken overload instead.
    /// </summary>
    public abstract Task HandleAsync();

    ///// <summary>
    ///// Optional overload that accepts a CancellationToken. Default implementation delegates to the parameterless HandleAsync.
    ///// Implement this overload in handlers that need cancellation support.
    ///// </summary>
    //public virtual Task HandleAsync(CancellationToken cancellationToken) => HandleAsync();

}//Int
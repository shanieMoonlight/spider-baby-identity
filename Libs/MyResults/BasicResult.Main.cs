using ExceptionHelpers;
using System.Text;

namespace MyResults;

public partial class BasicResult
{
    protected const string _unknownErrorMessage = "Something went wrong ;[";

    //------------------------------------//

    // Base constructor that everything ultimately calls
    protected BasicResult(
        bool succeeded,
        string? info = null,
        Exception? exception = null,
        ResultStatus status = ResultStatus.Success,
        object? badRequestResponse = null)
    {
        Succeeded = (exception == null) && succeeded;  // If we have an exception then we failed
        Info = info ?? string.Empty;
        Exception = exception;
        Status = status;
        BadRequestResponse = badRequestResponse;
    }

    //------------------------------------//

    /// <summary>
    /// Whether the operation succeeded or failed.
    /// </summary>
    public bool Succeeded { get; init; }

    /// <summary>
    /// Details of validation failure.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Any extra info needed.
    /// </summary>
    public string Info { get; init; } = string.Empty;

    /// <summary>
    /// The status of the result.
    /// </summary>
    public ResultStatus Status { get; init; } = ResultStatus.Success;

    /// <summary>
    /// Additional data for BadRequest responses
    /// </summary>
    public object? BadRequestResponse { get; init; }

    // Computed properties for backward compatibility

    /// <summary>
    /// Whether the item was not found
    /// </summary>
    public bool NotFound => Status == ResultStatus.NotFound;

    /// <summary>
    /// Whether the request was unauthorized
    /// </summary>
    public bool Unauthorized => Status == ResultStatus.Unauthorized;

    /// <summary>
    /// Whether access is forbidden
    /// </summary>
    public bool Forbidden => Status == ResultStatus.Forbidden;

    /// <summary>
    /// Whether the request was malformed
    /// </summary>
    public bool BadRequest => Status == ResultStatus.BadRequest;

    /// <summary>
    /// Whether a precondition is required
    /// </summary>
    public bool PreconditionRequired => Status == ResultStatus.PreconditionRequired;


    //------------------------------------//

    /// <summary>
    /// This is for subclasses to call, when converting to a BasicResult
    /// Create a BasicResult with the given parameters
    /// </summary>
    protected static BasicResult CreateBasicResult(
        bool succeeded = true,
        string? info = null,
        Exception? exception = null,
        ResultStatus status = ResultStatus.Success,
        object? badRequestResponse = null) =>
        new(
            succeeded: succeeded,
            info: info,
            exception: exception,
            status: status,
            badRequestResponse: badRequestResponse);


    //------------------------------------//

    public override string ToString()
    {
        var sb = new StringBuilder()
            .AppendLine($"{nameof(Status)}: {Status}")
            .AppendLine($"{nameof(Info)}: {Info}")
            .AppendLine($"{nameof(Status)}: {Status}");

        if (BadRequestResponse != null)
            sb.AppendLine($"{nameof(BadRequestResponse)}: {BadRequestResponse}");

        if (Exception != null)
            sb.AppendLine($"{nameof(Exception)}: {Exception.ToLogString()}");
        if (Exception?.InnerException != null)
            sb.AppendLine($"{nameof(Exception.InnerException)}: {Exception.InnerException.ToLogString()}");

        return sb.ToString();
    }

}//Cls

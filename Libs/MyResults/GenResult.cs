using ExceptionHelpers;
using System.Text;

namespace MyResults;

/// <summary>
/// Class for encapsulating the results of various maneuvers. 
/// </summary>
/// <typeparam name="T">The payload on a successful result</typeparam>
public partial class GenResult<T> : BasicResult
{

    // Base constructor that everything ultimately calls
    private GenResult(
        bool succeeded,
        T? value = default,
        string? info = null,
        Exception? exception = null,
        ResultStatus status = ResultStatus.Success,
        object? badRequestResponse = null)
        : base(succeeded, info: info, exception: exception, status: status, badRequestResponse: badRequestResponse)
    {
        Value = value;
    }

    //------------------------------------//

    /// <summary>
    /// The payload. Will probably be null if Succeeded is false
    /// </summary>
    public T? Value { get; init; }

    //------------------------------------//

    /// <summary>
    /// Converts this result into the <typeparamref name="TNew"/> equivalent
    /// Everything is the same except the value
    /// </summary>
    /// <typeparam name="TNew">New type</typeparam>
    /// <param name="newValue">New value if one exists</param>
    /// <returns>GenResult of type <typeparamref name="TNew"/></returns>
    public GenResult<TNew> Convert<TNew>(TNew? newValue = default, string? newInfo = null) =>
        new(Succeeded, newValue, newInfo ?? Info, Exception, Status, BadRequestResponse);

    //- - - - - - - - - - - - - - - - - - //        

    /// <summary>
    /// Converts this result into the <typeparamref name="TNew"/> equivalent
    /// Everything is the same except the value, which is transformed by the provided function
    /// </summary>
    /// <typeparam name="TNew">New type</typeparam>
    /// <param name="newValueConverter">Function to convert the value</param>
    /// <returns>GenResult of type <typeparamref name="TNew"/></returns>
    public GenResult<TNew> Convert<TNew>(Func<T?, TNew?> newValueConverter)
    {
        TNew? newValue = default;
        if (newValueConverter is not null && Value is not null)
            newValue = newValueConverter.Invoke(Value);

        return new GenResult<TNew>(Succeeded, newValue, Info, Exception, Status, BadRequestResponse);
    }

    //------------------------------------//

    public BasicResult ToBasicResult(string? customInfo = null) =>
       CreateBasicResult(
           succeeded: Succeeded,
           exception: Exception,
           info: customInfo ?? Info,
           status: Status,
           badRequestResponse: BadRequestResponse
       );

    //------------------------------------//

    public override string ToString()
    {
        var sb = new StringBuilder()
            .AppendLine($"{nameof(GenResult<T>)}: {(Succeeded ? "Success" : "Failure")}")
            .AppendLine($"{nameof(Info)}: {Info}")
            .AppendLine($"{nameof(Succeeded)}: {Succeeded}")
            .AppendLine($"{nameof(Status)}: {Status}");

        if (Value != null)
            sb.AppendLine($"{nameof(Value)}: {Value}");

        if (Exception != null)
            sb.AppendLine($"{nameof(Exception)}: {Exception.ToLogString()}");
        if (Exception?.InnerException != null)
            sb.AppendLine($"{nameof(Exception.InnerException)}: {Exception.InnerException.ToLogString()}");

        return sb.ToString();
    }

    //------------------------------------//

}

namespace Pagination.Result;
internal class PgResult<T>
{
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
    /// The payload, Always non-null on Succeeded
    /// </summary>
    public T? Value { get; init; }


    //-----------------------------// 

    private PgResult(bool succeeded, string? info, T? value, Exception? ex)
    {
        Succeeded = succeeded;
        Info = info ?? string.Empty;
        Value = value;
        Exception = ex;
    }

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Create successful result
    /// </summary>
    /// <param name="value">Payload</param>
    public PgResult(T value)
    {
        Value = value;
        Succeeded = true;
    }

    //- - - - - - - - - - - - - - - - - - //


    /// <summary>
    /// Create failed result
    /// </summary>
    /// <param name="exception">Exception detailing failure</param>
    public PgResult(Exception? exception, string? info)
    {
        Succeeded = false;
        Exception = exception;
        Info = info ?? string.Empty;
    }

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Create result 
    /// </summary>
    /// <param name="succeeded"></param>
    /// <param name="info"></param>
    public PgResult(bool succeeded, string? info)
    {
        Succeeded = succeeded;
        Info = info ?? string.Empty;
    }

    //-----------------------------// 

    /// <summary>
    /// Generates a PhResult with Succeeded set to false
    /// </summary>
    /// <param name="info">Why it failed</param>
    /// <returns>Failed result</returns>
    public static PgResult<T> Failure(string? info = null) => new(false, info);

    //- - - - - - - - - - - - - - - - - - // 

    /// <summary>
    /// Generates a PhResult with Succeeded set to false
    /// </summary>
    /// <param name="info">Why it failed</param>
    /// <returns>Failed result</returns>
    public static PgResult<T> Failure(Exception? ex, string? info = null) => new(ex, info);

    //-----------------------------//    

    /// <summary>
    /// Converts this result in to the <typeparamref name="TNew"/> Equivilant
    /// Everything is the same except the value
    /// </summary>
    /// <typeparam name="TNew">New type</typeparam>
    /// <param name="newValue">New value if one exists</param>
    /// <returns>PhResult of type <typeparamref name="TNew"/></returns>
    public PgResult<TNew> Convert<TNew>() =>
        new(Succeeded, Info, default, Exception);

}

namespace MyResults;
public partial class GenResult<T> : BasicResult
{

    /// <summary>
    /// Generates a GenResult with BadRequest status
    /// </summary>
    /// <param name="info">What is wrong with request</param>
    /// <returns>GenResult with BadRequest status</returns>
    public static new GenResult<T> BadRequestResult(string? info) =>
        new(
            succeeded: false,
            value: default,
            info: info,
            exception: null,
            status: ResultStatus.BadRequest
        );

    //------------------------------------//

    /// <summary>
    /// Generates a GenResult with BadRequest status
    /// </summary>
    /// <returns>GenResult with BadRequest status</returns>
    public static new GenResult<T> BadRequestResult(object badRequestResponse) =>
        new(false, default, _unknownErrorMessage, null, ResultStatus.BadRequest, badRequestResponse);

    //------------------------------------//

    /// <summary>
    /// Generates a GenResult with NotFound status
    /// </summary>
    /// <param name="info">Why it wasn't found</param>
    /// <returns>GenResult with NotFound status</returns>
    public static new GenResult<T> NotFoundResult(string? info = null) =>
        new(
            succeeded: false,
            value: default,
            info: info,
            exception: null,
            status: ResultStatus.NotFound
        );

    //------------------------------------//

    /// <summary>
    /// Generates a GenResult with Unauthorized status
    /// </summary>
    /// <param name="info">Why it wasn't authorized</param>
    /// <returns>GenResult with Unauthorized status</returns>
    public static new GenResult<T> UnauthorizedResult(string? info = null) =>
        new(
            succeeded: false,
            value: default,
            info: info,
            exception: null,
            status: ResultStatus.Unauthorized
        );

    //------------------------------------//

    /// <summary>
    /// Generates a Result with Forbidden status
    /// </summary>
    /// <param name="info">Why it was forbidden</param>
    /// <returns>Result with Forbidden status</returns>
    public static new GenResult<T> ForbiddenResult(string? info = null) =>
        new(
            succeeded: false,
            value: default,
            info: info,
            exception: null,
            status: ResultStatus.Forbidden
        );

    //------------------------------------//

    /// <summary>
    /// Generates a Result with PreconditionRequired status
    /// </summary>
    /// <param name="info">Any additional information</param>
    /// <returns>GenResult with PreconditionRequired status</returns>
    public static new GenResult<T> PreconditionRequiredResult(string? info = null) =>
        new(
            succeeded: false,
            value: default,
            info: info ?? _unknownErrorMessage,
            exception: null,
            status: ResultStatus.PreconditionRequired
        );

    //------------------------------------//

    /// <summary>
    /// Generates a GenResult with Succeeded set to false
    /// </summary>
    /// <param name="info">Why it failed</param>
    /// <returns>Failed result</returns>
    public static new GenResult<T> Failure(string? info = null) =>
        new(
            succeeded: false,
            value: default,
            info: info,
            exception: null,
            status: ResultStatus.Failure
        );

    //- - - - - - - - - - - - - - - - - - // 

    /// <summary>
    /// Generates a GenResult with Succeeded set to false
    /// </summary>
    /// <param name="ex">Exception detailing failure</param>
    /// <returns>Failed result</returns>
    public static GenResult<T> Failure(Exception ex) =>
        new(
            succeeded: false,
            value: default,
            info: ex.Message,
            exception: ex,
            status: ResultStatus.Failure
        );

    //- - - - - - - - - - - - - - - - - - //  

    /// <summary>
    /// Generates a GenResult with Succeeded set to false
    /// </summary>
    /// <param name="ex">Exception detailing failure</param>
    /// <param name="info">Why it failed</param>
    /// <returns>Failed result</returns>
    public static new GenResult<T> Failure(Exception ex, string info) =>
        new(
            succeeded: false,
            value: default,
            info: info ?? ex.Message,
            exception: ex,
            status: ResultStatus.Failure
        );

    //- - - - - - - - - - - - - - - - - - //     

    /// <summary>
    /// Generates a GenResult with Succeeded set to false
    /// </summary>
    /// <param name="ex">Exception detailing failure</param>
    /// <param name="info">Why it failed</param>
    /// <returns>Failed result</returns>
    public static GenResult<T> Failure(ResultStatus status, Exception? ex, string? info) =>
        new(
            succeeded: false,
            value: default,
            info: info ?? _unknownErrorMessage,
            exception: ex,
            status: status
        );


    //------------------------------------// 

    /// <summary>
    /// Generates a GenResult with Succeeded set to true
    /// </summary>
    /// <param name="value">The result value</param>
    /// <returns>Succeeded result with value</returns>
    public static GenResult<T> Success(T value, string? info = null) =>
       new(
            succeeded: true,
            value: value,
            info: info,
            exception: null,
            status: ResultStatus.Success
        );




}//Cls

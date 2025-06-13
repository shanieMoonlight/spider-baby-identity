namespace MyResults;
public partial class BasicResult
{

    /// <summary>
    /// Generates a BasicResult with Succeeded set to false
    /// </summary>
    /// <param name="info">Why it failed</param>
    /// <returns>Failed result</returns>
    public static BasicResult Failure(string? info = null) =>
        new(succeeded: false, info: info ?? _unknownErrorMessage, status: ResultStatus.Failure);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Generates a BasicResult with Succeeded set to false
    /// </summary>
    /// <param name="ex">Exception detailing failure</param>
    /// <param name="info">Why it failed</param>
    /// <returns>Failed result</returns>
    public static BasicResult Failure(Exception ex, string? info = null) =>
        new(succeeded: false, exception: ex, info: info ?? ex.Message, status: ResultStatus.Failure);


    //------------------------------------// 


    /// <summary>
    /// Generates a BasicResult with Succeeded set to true
    /// </summary>
    /// <param name="info">Additional information</param>
    /// <returns>Succeeded result</returns>
    public static BasicResult Success(string? info = null) =>
        new(succeeded: true, info: info, status: ResultStatus.Success);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Create empty successful result
    /// </summary>
    public static BasicResult Success() =>
        new(succeeded: true, status: ResultStatus.Success);

    //====================================//

    /// <summary>
    /// Generates a BasicResult with BadRequest status
    /// </summary>
    /// <param name="info">What is wrong with request</param>
    /// <returns>Result with BadRequest status</returns>
    public static BasicResult BadRequestResult(string? info) =>
        new(
            succeeded: false,
            info: info ?? _unknownErrorMessage,
            status: ResultStatus.BadRequest
        );

    //------------------------------------// 

    /// <summary>
    /// Generates a BasicResult with BadRequest status
    /// </summary>
    /// <param name="badRequestResponse">Detailed response about the bad request</param>
    /// <returns>Result with BadRequest status</returns>
    public static BasicResult BadRequestResult(object badRequestResponse) =>
        new(
            succeeded: false,
            status: ResultStatus.BadRequest,
            badRequestResponse: badRequestResponse
        );

    //------------------------------------//   

    /// <summary>
    /// Generates a BasicResult with PreconditionRequired status
    /// </summary>
    /// <param name="info">Any additional information</param>
    /// <returns>Result with PreconditionRequired status</returns>
    public static BasicResult PreconditionRequiredResult(string? info = null) =>
        new(
            succeeded: false,
            info: info ?? _unknownErrorMessage,
            status: ResultStatus.PreconditionRequired
        );

    //------------------------------------//

    /// <summary>
    /// Generates a Result with NotFound status
    /// </summary>
    /// <param name="info">Why it wasn't found</param>
    /// <returns>Result with NotFound status</returns>
    public static BasicResult NotFoundResult(string? info = null) =>
        new(
            succeeded: false,
            info: info,
            status: ResultStatus.NotFound
        );

    //------------------------------------//

    /// <summary>
    /// Generates a Result with Unauthorized status
    /// </summary>
    /// <param name="info">Why the user is not authorized</param>
    /// <returns>Result with Unauthorized status</returns>
    public static BasicResult UnauthorizedResult(string? info = null) =>
        new(
            succeeded: false,
            info: info,
            status: ResultStatus.Unauthorized
        );

    //------------------------------------//

    /// <summary>
    /// Generates a Result with Forbidden status
    /// </summary>
    /// <param name="info">Why access is forbidden</param>
    /// <returns>Result with Forbidden status</returns>
    public static BasicResult ForbiddenResult(string? info = null) =>
        new(
            succeeded: false,
            info: info,
            status: ResultStatus.Forbidden
        );
}//Cls
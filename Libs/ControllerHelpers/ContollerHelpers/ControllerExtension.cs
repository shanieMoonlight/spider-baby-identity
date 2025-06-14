using ControllerHelpers.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyResults;
using StringHelpers;

namespace ControllerHelpers;


//##################################       ControllerBase      ########################################//

public static class ControllerExtension
{
    /// <summary>
    /// Generates an OK Result or an Internal Server Error Result including the value, depending on success
    /// </summary>
    /// <param name="controller">Controller</param>
    /// <param name="result">Object containing data and success info</param>
    /// <param name="successMessage">What to say when it works out. If null will use result.Value</param>
    /// <returns>OK Result or an Internal Server Error Result</returns>
    public static ObjectResult ProcessResult(this ControllerBase controller, BasicResult result, string? successMessage = null)
    {
        if (!result.Succeeded)
            return controller.ProcessFailedResult(result);

        return !successMessage.IsNullOrWhiteSpace()
            ? controller.Ok(MessageResponseDto.Generate(successMessage!))
            : controller.Ok(MessageResponseDto.Generate(result.Info ?? nameof(BasicResult.Succeeded)));

    }

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates an OK Result or an Internal Server Error Result including the value, depending on success
    /// </summary>
    /// <typeparam name="T">What's contained in the GenResult</typeparam>
    /// <param name="controller">Controller</param>
    /// <param name="result">Object containing data and success info</param>
    /// <param name="successMessage">What to say when it works out. If null will use result.Value</param>
    /// <returns>OK Result or an Internal Server Error Result</returns>
    public static ObjectResult ProcessResult<T>(this ControllerBase controller, GenResult<T> result, string? successMessage = null)
    {
        if (!result.Succeeded)
            return controller.ProcessFailedResult(result);

        return !successMessage.IsNullOrWhiteSpace()
            ? controller.Ok(MessageResponseDto.Generate(successMessage!))
            : controller.Ok(result.Value);

    }

    //-----------------------------------//

    /// <summary>
    /// Generates an OK Result or an Internal Server Error Result including the value, depending on success
    /// </summary>
    /// <param name="controller">Controller</param>
    /// <param name="result">Object containing data and success info</param>
    /// <param name="logger">Logger being used in controller</param>
    /// <param name="successMessage">What to say when it works out. If null will use result.Value</param>
    /// <returns>OK Result or an Internal Server Error Result</returns>
    public static ObjectResult ProcessResult(this ControllerBase controller, BasicResult result, ILogger logger, string? successMessage = null)
    {

        if (!result.Succeeded)
            return controller.ProcessFailedResult(result, logger);

        return !successMessage.IsNullOrWhiteSpace()
            ? controller.Ok(MessageResponseDto.Generate(successMessage!))
            : controller.Ok(MessageResponseDto.Generate(result.Info ?? nameof(BasicResult.Succeeded)));

    }

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates an OK Result or an Internal Server Error Result including the value, depending on success
    /// </summary>
    /// <typeparam name="T">What's contained in the GenResult</typeparam>
    /// <param name="controller">Controller</param>
    /// <param name="result">Object containing data and success info</param>
    /// <param name="logger">Logger being used in controller</param>
    /// <param name="successMessage">What to say when it works out. If null will use result.Value</param>
    /// <returns>OK Result or an Internal Server Error Result</returns>
    public static ObjectResult ProcessResult<T>(this ControllerBase controller, GenResult<T> result, ILogger logger, string? successMessage = null)
    {
        if (!result.Succeeded)
            return controller.ProcessFailedResult(result, logger);

        if (!successMessage.IsNullOrWhiteSpace())
            return controller.Ok(MessageResponseDto.Generate(successMessage!));

        return typeof(T) == typeof(string)
            ? controller.Ok(MessageResponseDto.Generate(result.Value?.ToString() ?? result.Info))
            : controller.Ok(result.Value);

    }

    //-----------------------------------//

    /// <summary>
    /// Generates an OK Result or an Internal Server Error Result including the value, depending on success
    /// </summary>
    /// <typeparam name="T">What's contained in the GenResult</typeparam>
    /// <param name="controller">Controller</param>
    /// <param name="result">Object containing data and success info</param>
    /// <param name="logger">Logger being used in controller</param>
    /// <param name="successConversion">Function to convert result.Value before emitting on seccess</param>
    /// <returns>OK Result or an Internal Server Error Result</returns>
    public static ObjectResult ProcessResult<T>(this ControllerBase controller, GenResult<T> result, ILogger logger, Func<T, object> successConversion) =>
        !result.Succeeded
            ? controller.ProcessFailedResult(result, logger)
            : controller.Ok(successConversion(result.Value!)); //Success is non-null

    //-----------------------------------//

    /// <summary>
    /// Generates an OK Result or an Internal Server Error Result including the value, depending on success
    /// </summary>
    /// <typeparam name="T">What's contained in the GenResult</typeparam>
    /// <param name="controller">Controller</param>
    /// <param name="result">Object containing data and success info</param>
    /// <param name="successConversion">Function to convert result.Value before emitting on seccess</param>
    /// <returns>OK Result or an Internal Server Error Result</returns>
    public static ObjectResult ProcessResult<T>(this ControllerBase controller, GenResult<T> result, Func<T, object> successConversion) =>
        !result.Succeeded
            ? controller.ProcessFailedResult(result)
            : controller.Ok(successConversion(result.Value!)); //Success is non-null so we can use ! here

    //-----------------------------------//

    /// <summary>
    /// Generates an ErrorResult (Depending on Result Flags)
    /// </summary>
    /// <param name="controller">Controller</param>
    /// <param name="result">Object containing data and success info</param>
    /// <returns>The appropriate error result</returns>
    public static ObjectResult ProcessFailedResult(this ControllerBase controller, BasicResult result) =>
        controller.GenerateObjectResult(result);

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates an ErrorResult (Depending on Result Flags)
    /// </summary>
    /// <param name="controller">Controller</param>
    /// <param name="result">Object containing data and success info</param>
    /// <param name="logger">Logger being used in controller</param>
    /// <returns>The appropriate error result</returns>
    public static ObjectResult ProcessFailedResult(this ControllerBase controller, BasicResult result, ILogger logger)
    {
        logger?.LogError("Result: {result}", result);
        return controller.GenerateObjectResult(result);
    }

    //-----------------------------------//

    /// <summary>
    /// Generates an ErrorResult (Depending on Result Flags) 
    /// </summary>
    /// <typeparam name="T">What's contained in the GenResult</typeparam>
    /// <param name="controller">Controller</param>
    /// <param name="result">Object containing data and success info</param>
    /// <param name="logger">Logger being used in controller</param>
    /// <returns>The appropriate error result</returns>
    public static ObjectResult ProcessFailedResult<T>(this ControllerBase controller, GenResult<T> result) =>
        controller.GenerateObjectResult<T>(result);

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates an ErrorResult (Depending on Result Flags) 
    /// </summary>
    /// <typeparam name="T">What's contained in the GenResult</typeparam>
    /// <param name="controller">Controller</param>
    /// <param name="result">Object containing data and success info</param>
    /// <param name="logger">Logger being used in controller</param>
    /// <returns>The appropriate error result</returns>
    public static ObjectResult ProcessFailedResult<T>(this ControllerBase controller, GenResult<T> result, ILogger logger)
    {
        logger?.LogError("Result: {result}", result);
        return controller.GenerateObjectResult<T>(result);
    }

    //-----------------------------------//

    public static ObjectResult GenerateObjectResult<T>(this ControllerBase controller, GenResult<T> result)
    {
        if (result.BadRequest)
        {
            if (result.BadRequestResponse is not null)
                return controller.BadRequest(result.BadRequestResponse);
            if (result.Value is not null)
                return controller.BadRequest(result.Value);
            else
                return controller.BadRequest(MessageResponseDto.Generate(result.Info));
        }

        if (result.PreconditionRequired) //Email confirmation or 2Factor Required. Might contain extra data
        {
            return result.Value != null
                ? controller.PreconditionRequired(result.Value)
                : controller.PreconditionRequiredWithMessageDto(result.Info);
        }

        if (result.NotFound)
            return controller.NotFound(MessageResponseDto.Generate(result.Info));

        if (result.Unauthorized)
            return controller.UnauthorizedWithMessageDto(result.Info);

        if (result.Forbidden)
            return controller.ForbiddenWithMessageDto(result.Info);

        return controller.InternalServerError(MessageResponseDto.Generate(result.Info));

    }

    //-----------------------------------//

    public static ObjectResult GenerateObjectResult(this ControllerBase controller, BasicResult result)
    {
        if (result.BadRequest)
        {
            return result.BadRequestResponse != null
                ? controller.BadRequest(result.BadRequestResponse)
                : controller.BadRequest(MessageResponseDto.Generate(result.Info));
        }

        if (result.NotFound)
            return controller.NotFound(MessageResponseDto.Generate(result.Info));

        if (result.PreconditionRequired)
            return controller.PreconditionRequiredWithMessageDto(result.Info);

        if (result.Unauthorized)
            return controller.UnauthorizedWithMessageDto(result.Info);

        if (result.Forbidden)
            return controller.ForbiddenWithMessageDto(result.Info);

        return controller.InternalServerError(MessageResponseDto.Generate(result.Info));

    }

    //-----------------------------------//

}//Cls
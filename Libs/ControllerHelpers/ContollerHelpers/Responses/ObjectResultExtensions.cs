using Microsoft.AspNetCore.Mvc;

namespace ControllerHelpers.Responses;


//Ignoring unused paramaters in some methods so that the user will have the same experience either way.
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
public static class ObjectResultExtensions
{
    //-----------------------------------//

    /// <summary>
    /// Generates an Ok Result including the data:  {message: msg}
    /// </summary>
    /// <returns>Ok Result (200)</returns>
    public static ObjectResult OkWithMessageDto(this ControllerBase controller, string msg) =>
        controller.Ok(MessageResponseDto.Generate(msg));

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates an Not Found Result including the data:  {message: msg}
    /// </summary>
    /// <returns>Not Found Error Result (404)</returns>
    public static ObjectResult NotFoundWithMessageDto(this ControllerBase controller, string msg) =>
        controller.NotFound(MessageResponseDto.Generate(msg));

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates an Unauthorized Result including the data:  {message: msg}
    /// </summary>
    /// <returns>Unauthorized Error Result (401)</returns>
    public static ObjectResult UnauthorizedWithMessageDto(this ControllerBase controller, string msg) =>
        controller.Unauthorized(MessageResponseDto.Generate(msg));

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates an Forbidden Result including the value
    /// </summary>
    /// <returns>Forbidden Error Result (403)</returns>
    public static ObjectResult ForbiddenWithMessageDto(this ControllerBase controller, string msg) =>
        new ForbiddenResponse(MessageResponseDto.Generate(msg));

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates an Precondition Required Result including the data:  {message: msg}
    /// </summary>
    /// <returns>Precondition Required Error Result (428)</returns>
    public static ObjectResult PreconditionRequiredWithMessageDto(this ControllerBase controller, string msg) =>
        new PreconditionRequiredResponse(MessageResponseDto.Generate(msg));

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates an Precondition Required Result including the data:  {message: value}
    /// </summary>
    /// <returns>Precondition Required Error Result (428)</returns>
    public static ObjectResult PreconditionRequired(this ControllerBase controller, object value) =>
        new PreconditionRequiredResponse(value);

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates an Internal Server Error Result including the value
    /// </summary>
    /// <returns>Internal Server Error Result (500)</returns>
    public static ObjectResult InternalServerError(this ControllerBase controller, object value) =>
        new InternalServerErrorResponse(value);

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Generates an Internal Server Result including the data:  {message: msg}
    /// </summary>
    /// <returns>Internal Server Error Result (500) </returns>
    public static ObjectResult InternalServerErrorWithMessageDto(this ControllerBase controller, string msg) =>
        new InternalServerErrorResponse(MessageResponseDto.Generate(msg));

    //-----------------------------------//

}//Cls

using ControllerHelpers;
using ControllerHelpers.Responses;
using ID.Application.Features.Images.Qry.FallbackLogo;
using ID.Application.Features.Images.Qry.PhoneConfirmed;
using ID.Application.Features.Images.Qry.Welcome;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyResults;
using ID.GlobalSettings.Routes;

namespace ID.Presentation.Controllers;

[ApiController]
[Route($"{IdRoutes.Base}/[controller]/[action]")]
public class IdImagesController(ISender sender) : Controller
{

    //------------------------//

    /// <summary>
    /// Returns the welcome image shown after email confirmation.
    /// </summary>
    /// <returns>The welcome image file or an error message.</returns>
    [HttpGet()]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> EmailConfirmed()
    {
        var imgResult = await sender.Send(new GetWelcomeImgQry());
        return ProcessImgResult(imgResult);
    }

    //------------------------//

    /// <summary>
    /// Returns the image shown after phone confirmation.
    /// </summary>
    /// <returns>The phone confirmed image file or an error message.</returns>
    [HttpGet()]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> PhoneConfirmed()
    {
        var imgResult = await sender.Send(new GetPhoneConfirmedImgQry());
        return ProcessImgResult(imgResult);
    }

    //------------------------//

    /// <summary>
    /// Returns the fallback logo image for the application.
    /// </summary>
    /// <returns>The fallback logo image file or an error message.</returns>
    [HttpGet()]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> FallbackLogo()
    {
        var imgResult = await sender.Send(new GetFallbackLogoQry());
        return ProcessImgResult(imgResult);
    }

    //------------------------//

    /// <summary>
    /// Processes the result of an image query and returns the appropriate file or error response.
    /// </summary>
    /// <param name="imgResult">The result of the image query.</param>
    /// <returns>The image file if successful, otherwise an error response.</returns>
    private ActionResult<MessageResponseDto> ProcessImgResult(BasicResult imgResult)
    {
        if (!imgResult.Succeeded)
            return this.ProcessFailedResult(imgResult);

        var imgPath = imgResult.Info;
        string ext = Path.GetExtension(imgPath);
        string filename = Path.GetFileName(imgPath);

        return PhysicalFile(imgPath, $"image/{ext}", filename);
    }


    //------------------------//

}//Cls

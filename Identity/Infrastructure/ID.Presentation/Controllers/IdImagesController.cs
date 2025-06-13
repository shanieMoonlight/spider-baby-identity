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

    [HttpGet()]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> EmailConfirmed()
    {
        var imgResult = await sender.Send(new GetWelcomeImgQry());
        return ProcessImgResult(imgResult);
    }

    //------------------------//

    [HttpGet()]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> PhoneConfirmed()
    {
        var imgResult = await sender.Send(new GetPhoneConfirmedImgQry());
        return ProcessImgResult(imgResult);
    }

    //------------------------//

    [HttpGet()]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> FallbackLogo()
    {
        var imgResult = await sender.Send(new GetFallbackLogoQry());
        return ProcessImgResult(imgResult);
    }

    //------------------------//

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

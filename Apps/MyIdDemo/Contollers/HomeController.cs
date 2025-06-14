using ControllerHelpers.Responses;
using ID.GlobalSettings.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyIdDemo.Contollers;

[ApiController]
[Route($"{IdRoutes.Base}/[controller]")]
[AllowAnonymous]
public class AardvaarkController : Controller
{
    [HttpGet("[action]")]
    public ActionResult<MessageResponseDto> Index()
    {
        return MessageResponseDto.Generate("Index");
    }

    [HttpGet]
    public ActionResult<MessageResponseDto> Get()
    {
        return MessageResponseDto.Generate("Get");
    }

    [HttpPost]
    public ActionResult<MessageResponseDto> Post()
    {
        return MessageResponseDto.Generate("Post");
    }


    [HttpPatch]
    public ActionResult<MessageResponseDto> Patch()
    {
        return MessageResponseDto.Generate("Patch");
    }

    [HttpDelete]
    public ActionResult<MessageResponseDto> Delete()
    {
        return MessageResponseDto.Generate("Delete");
    }
}

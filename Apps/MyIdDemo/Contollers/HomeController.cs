using CollectionHelpers;
using ControllerHelpers.Responses;
using ID.GlobalSettings.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyIdDemo.Contollers;

public interface IWeatherForecastList
{
    List<WeatherForecast> WeatherForecasts { get; set; }
}


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
    public ActionResult<List<WeatherForecast>> Get()
    {
        return new List<WeatherForecast>() {
            new() {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                TemperatureC = -3,
                Summary = "Freezing"
            },
            new() {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                TemperatureC = 2,
                Summary = "Bracing"
            },
            new() {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                TemperatureC = 15,
                Summary = "Mild"
            },
            new() {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(4)),
                TemperatureC = 23,
                Summary = "Warm"
            },
            new() {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                TemperatureC = 32,
                Summary = "Hot"
            }
        };
    }


    [HttpGet("{userId}")]
    public ActionResult<MessageResponseDto> Get(Guid userId)
    {
        return MessageResponseDto.Generate($"Get: {userId}");
    }


    [HttpPost]
    public ActionResult<MessageResponseDto> Post(WeatherForecast forcast)
    {
        return MessageResponseDto.Generate($"Post: {forcast}");
    }


    [HttpPatch]
    public ActionResult<MessageResponseDto> Patch(IWeatherForecastList list)
    {
        return MessageResponseDto.Generate($"Patch: {list.WeatherForecasts.JoinStr(", ")}");
    }



    [HttpDelete]
    public ActionResult<MessageResponseDto> Delete(int id)
    {
        return MessageResponseDto.Generate($"Delete: {id}");
    }


}

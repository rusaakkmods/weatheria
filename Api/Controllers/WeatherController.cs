using MediatR;
using Microsoft.AspNetCore.Mvc;
using Weatheria.Application.Features.Weather;

namespace Weatheria.Api.Controllers;

[ApiController]
[Route("api/weather")]
public sealed class WeatherController(ISender sender) : ControllerBase
{
    [HttpGet("{cityName}")]
    public async Task<ActionResult<WeatherDto>> Get(
        string cityName,
        CancellationToken cancellationToken)
    {
        var weather = await sender.Send(
            new GetWeatherQuery(cityName),
            cancellationToken);

        return Ok(weather);
    }
}
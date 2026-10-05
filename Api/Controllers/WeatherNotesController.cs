using MediatR;
using Microsoft.AspNetCore.Mvc;
using Weatheria.Application.Features.WeatherNotes;

namespace Weatheria.Api.Controllers;

[ApiController]
[Route("api/weather/notes")]
public sealed class WeatherNotesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateWeatherNoteRequest request, CancellationToken cancellationToken)
    {
        var id = await sender.Send(
            new CreateWeatherNoteCommand(
                request.City,
                request.Content),
                cancellationToken);
        
        return Created($"/api/weather/notes/{id}", new CreateWeatherNoteResponse(id));
    }

    public sealed record CreateWeatherNoteRequest(string City, string Content);

    public sealed record CreateWeatherNoteResponse(Guid Id);
}
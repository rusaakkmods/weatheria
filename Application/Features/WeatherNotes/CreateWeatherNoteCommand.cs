using MediatR;

namespace Weatheria.Application.Features.WeatherNotes;

public sealed record CreateWeatherNoteCommand(string City, string Content) : IRequest<Guid>;

using MediatR;
using Weatheria.Application.Abstractions.Persistence;
using Weatheria.Domain.Entities;

namespace Weatheria.Application.Features.WeatherNotes;

public sealed class CreateWeatherNoteCommandHandler(IWeatherNoteRepository repository): IRequestHandler<CreateWeatherNoteCommand, Guid>
{
    public async Task<Guid> Handle(CreateWeatherNoteCommand request, CancellationToken cancellationToken)
    {
        var note = WeatherNote.Create(request.City, request.Content);

        await repository.AddAsync(note, cancellationToken);

        return note.Id;
    }
}
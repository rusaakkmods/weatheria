using Weatheria.Domain.Entities;

namespace Weatheria.Application.Abstractions.Persistence;

public interface IWeatherNoteRepository
{
    Task AddAsync(WeatherNote note, CancellationToken cancellationToken);
    Task<WeatherNote?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    
}
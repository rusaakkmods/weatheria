using Microsoft.EntityFrameworkCore;
using Weatheria.Application.Abstractions.Persistence;
using Weatheria.Domain.Entities;

namespace Weatheria.Infrastructure.Persistence.Repositories;

public sealed class WeatherNoteRepository(WeatheriaDbContext dbContext) : IWeatherNoteRepository
{
    public async Task AddAsync(WeatherNote note, CancellationToken cancellationToken)
    {
        await dbContext.WeatherNotes.AddAsync(note, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<WeatherNote?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.WeatherNotes
            .AsNoTracking()
            .SingleOrDefaultAsync(note => note.Id == id, cancellationToken);
    }
}
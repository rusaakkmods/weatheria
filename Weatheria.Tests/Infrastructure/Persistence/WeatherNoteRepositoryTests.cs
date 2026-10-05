using FluentAssertions;
using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Weatheria.Domain.Entities;
using Weatheria.Infrastructure.Persistence;
using Weatheria.Infrastructure.Persistence.Repositories;

namespace Weatheria.Tests.Infrastructure.Persistence;

public class WeatherNoteRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldPersistWeatherNote()
    {
        // Arrange
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<WeatheriaDbContext>()
        .UseSqlite(connection)
        .Options;

        await using var dbContext = new WeatheriaDbContext(options);

        await dbContext.Database.EnsureCreatedAsync(); // isolated tes

        var repository = new WeatherNoteRepository(dbContext);

        var note = WeatherNote.Create(
            "Bandung", "Hujan Deras");
        
        await repository.AddAsync(
            note,
            CancellationToken.None);

        var saved = await repository.GetByIdAsync(
            note.Id,
            CancellationToken.None
        );

        // Assert
        saved.Should().NotBeNull();
        saved.Id.Should().Be(note.Id);
        saved.City.Should().Be("Bandung");
        saved.Content.Should().Be("Hujan Deras");


    }
}
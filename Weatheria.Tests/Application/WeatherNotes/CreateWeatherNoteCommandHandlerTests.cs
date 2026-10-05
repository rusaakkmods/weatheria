using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Weatheria.Application.Abstractions.Persistence;
using Weatheria.Application.Features.WeatherNotes;
using Weatheria.Domain.Entities;
using Weatheria.Infrastructure.Persistence;
using Weatheria.Infrastructure.Persistence.Repositories;
using Xunit;

namespace Weatheria.Tests.Application.WeatherNotes;

public class CreateWeatherNoteCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithSqliteRepository_ShouldPersistWeatherNote()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<WeatheriaDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new WeatheriaDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var repository = new WeatherNoteRepository(dbContext);
        var handler = new CreateWeatherNoteCommandHandler(repository);
        var command = new CreateWeatherNoteCommand("Bandung", "Panas Belentrang");

        var id = await handler.Handle(command, CancellationToken.None);

        dbContext.ChangeTracker.Clear();

        var saved = await repository.GetByIdAsync(id, CancellationToken.None);

        saved.Should().NotBeNull();
        saved!.City.Should().Be(command.City);
        saved.Content.Should().Be(command.Content);
    }

    [Fact]
    public async Task Handle_ShouldCreateAndPersistWeatherNote()
    {
        var repository = new Mock<IWeatherNoteRepository>();

        var command = new CreateWeatherNoteCommand(
            "Bandung",
            "Panas Belentrang");
        
        var handler = new CreateWeatherNoteCommandHandler(repository.Object);

        // act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        repository.Verify(
            x => x.AddAsync(
                It.Is<WeatherNote>(note =>
                note.Id == result
                && note.City == "Bandung"
                && note.Content == "Panas Belentrang"),
                It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
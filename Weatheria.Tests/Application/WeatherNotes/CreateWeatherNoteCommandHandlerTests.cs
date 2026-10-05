using FluentAssertions;
using Moq;
using Weatheria.Application.Abstractions.Persistence;
using Weatheria.Application.Features.WeatherNotes;
using Weatheria.Domain.Entities;
using Xunit;

namespace Weatheria.Tests.Application.WeatherNotes;

public class CreateWeatherNoteCommandHandlerTests
{
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
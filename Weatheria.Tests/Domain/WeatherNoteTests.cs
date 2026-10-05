using FluentAssertions;
using Xunit;
using Weatheria.Domain.Entities;

namespace Weatheria.Tests.Domain;

public class WeatherNoteTests
{
    [Fact]
    public void CreateWeatherNote_ShouldCreateWeatherNote()
    {
        // Arrange
        var city = "Bandung";
        var content = "Rainy afternoon";

        // Act
        var note = WeatherNote.Create(city, content);

        // Assert
        note.Should().NotBeNull();
        note.City.Should().Be(city);
        note.Content.Should().Be(content);
        note.Id.Should().NotBeEmpty();
        note.CreatedAt.Should().NotBe(default);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldRejectEmptyCity(string city)
    {
        // Act
        var act = () => WeatherNote.Create(city, "Hujan deras");
        
        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldRejectEmptyContent(string content)
    {
        // Act
        var act = () => WeatherNote.Create("Bandung", content);
        
        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
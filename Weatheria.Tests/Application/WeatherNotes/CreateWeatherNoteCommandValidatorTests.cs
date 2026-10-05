using System.Xml.Serialization;
using FluentAssertions;
using Weatheria.Application.Features.WeatherNotes;
using Xunit;

namespace Weatheria.Tests.Application.WeatherNotes;

public class CreateWeatherNoteCommandValidatorTests
{
    [Fact]
    public void Validate_ShouldRejectEmptyCity()
    {
        var validator = new CreateWeatherNoteCommandValidator();

        var command = new CreateWeatherNoteCommand("","Hujan");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "City");
    }

    [Fact]
    public void Validate_ShouldRejectEmptyContent()
    {
        var validator = new CreateWeatherNoteCommandValidator();

        var command = new CreateWeatherNoteCommand("Bandung", "");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Content");
    }
}
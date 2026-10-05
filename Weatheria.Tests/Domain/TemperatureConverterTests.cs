using FluentAssertions;
using Xunit;
using Weatheria.Domain.Services;

namespace Weatheria.Tests.Domain;

public class TemperatureConverterTests
{
    [Theory]
    [InlineData(32, 0)]
    [InlineData(212, 100)]
    [InlineData(-40, -40)]
    public void ConvertFahrenheitToCelsius_ShouldReturnExpectedResult(double fahrenheit, double expectedCelsius)
    {
        // Act
        var result = TemperatureConverter.ConvertFahrenheitToCelsius(fahrenheit);

        // Assert
        result.Should().BeApproximately(expectedCelsius, 0.001);
    }
}
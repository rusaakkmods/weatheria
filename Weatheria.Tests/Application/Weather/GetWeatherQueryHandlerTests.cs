using FluentAssertions;
using Moq;
using Weatheria.Application.Abstractions;
using Weatheria.Application.Features.Weather;
using Xunit;

namespace Weatheria.Tests.Application.Weather;

public class GetWeatherQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnWeatherAndConvertFahrenheitToCelsius()
    {
        var observationTime = new DateTimeOffset(2026, 10, 5, 12, 0, 0, TimeSpan.Zero);
        var observation = new WeatherObservation(
            "Tokyo",
            "JP",
            observationTime,
            4.5,
            270,
            10000,
            1013,
            "clear sky",
            50,
            40,
            60);
        var weatherService = new Mock<IWeatherService>();
        var cancellationToken = new CancellationTokenSource().Token;

        weatherService
            .Setup(service => service.GetWeatherAsync("Tokyo", cancellationToken))
            .ReturnsAsync(observation);

        var handler = new GetWeatherQueryHandler(weatherService.Object);

        var result = await handler.Handle(
            new GetWeatherQuery("Tokyo"),
            cancellationToken);

        result.Should().BeEquivalentTo(new WeatherDto(
            "Tokyo",
            "JP",
            observationTime,
            4.5,
            270,
            10000,
            1013,
            "clear sky",
            50,
            10,
            40,
            60));
    }
}
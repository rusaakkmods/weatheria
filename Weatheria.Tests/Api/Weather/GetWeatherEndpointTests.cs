using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Moq;
using Weatheria.Application.Abstractions;
using Weatheria.Application.Features.Weather;
using Xunit;

namespace Weatheria.Tests.Api.Weather;

public class GetWeatherEndpointTests
{
    [Fact]
    public async Task Get_WhenWeatherServiceFails_ShouldReturnProblemDetailsWithoutExceptionDetails()
    {
        const string internalError = "OpenWeather credential=secret-marker";
        var weatherService = new Mock<IWeatherService>();
        weatherService
            .Setup(service => service.GetWeatherAsync("Tokyo", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException(internalError));

        await using var factory = new WeatheriaWebApplicationFactory(weatherService.Object);
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/weather/Tokyo");
        var responseBody = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
        responseBody.Should().NotContain(internalError);
        responseBody.Should().NotContain("secret-marker");

        using var problem = JsonDocument.Parse(responseBody);
        problem.RootElement.GetProperty("status").GetInt32()
            .Should().Be((int)HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task Get_ShouldReturnWeatherFromFakeService()
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
        weatherService
            .Setup(service => service.GetWeatherAsync("Tokyo", It.IsAny<CancellationToken>()))
            .ReturnsAsync(observation);

        await using var factory = new WeatheriaWebApplicationFactory(weatherService.Object);
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/weather/Tokyo");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var weather = await response.Content.ReadFromJsonAsync<WeatherDto>();

        weather.Should().BeEquivalentTo(new WeatherDto(
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
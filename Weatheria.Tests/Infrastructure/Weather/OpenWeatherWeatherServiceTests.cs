using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Weatheria.Application.Features.Weather;
using Weatheria.Infrastructure.Weather;
using Xunit;

namespace Weatheria.Tests.Infrastructure.Weather;

public class OpenWeatherWeatherServiceTests
{
    [Fact]
    public async Task GetWeatherAsync_ShouldMapImperialResponseWithoutNetwork()
    {
        var observationTime = new DateTimeOffset(2026, 10, 5, 12, 0, 0, TimeSpan.Zero);
        var responseBody = JsonSerializer.Serialize(new
        {
            name = "Tokyo",
            sys = new { country = "JP" },
            dt = observationTime.ToUnixTimeSeconds(),
            wind = new { speed = 4.5, deg = 270 },
            visibility = 10000,
            main = new { temp = 50.0, pressure = 1013, humidity = 60 },
            weather = new[] { new { description = "clear sky" } }
        });
        var handler = new StubHttpMessageHandler(responseBody);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.openweathermap.org/")
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenWeather:ApiKey"] = "test-key"
            })
            .Build();
        var service = new OpenWeatherWeatherService(httpClient, configuration);

        var observation = await service.GetWeatherAsync("Tokyo", CancellationToken.None);

        handler.RequestUri.Should().NotBeNull();
        handler.RequestUri!.AbsolutePath.Should().Be("/data/2.5/weather");
        handler.RequestUri.Query.Should().Contain("q=Tokyo");
        handler.RequestUri.Query.Should().Contain("appid=test-key");
        handler.RequestUri.Query.Should().Contain("units=imperial");
        observation.Should().BeEquivalentTo(new WeatherObservation(
            "Tokyo",
            "JP",
            observationTime,
            4.5,
            270,
            10000,
            1013,
            "clear sky",
            50,
            observation.DewPointFahrenheit,
            60));
        observation.DewPointFahrenheit.Should().BeApproximately(36.6, 0.5);
    }

    private sealed class StubHttpMessageHandler(string responseBody) : HttpMessageHandler
    {
        public Uri? RequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            });
        }
    }
}
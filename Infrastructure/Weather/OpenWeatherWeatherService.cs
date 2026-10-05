using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Weatheria.Application.Abstractions;
using Weatheria.Application.Features.Weather;
using Weatheria.Domain.Services;

namespace Weatheria.Infrastructure.Weather;

public sealed class OpenWeatherWeatherService(
    HttpClient httpClient,
    IConfiguration configuration) : IWeatherService
{
    public async Task<WeatherObservation> GetWeatherAsync(
        string cityName,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cityName);

        var apiKey = configuration["OpenWeather:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("OpenWeather:ApiKey is not configured.");
        }

        var requestUri = $"data/2.5/weather?q={Uri.EscapeDataString(cityName)}" +
            $"&appid={Uri.EscapeDataString(apiKey)}&units=imperial";

        using var response = await httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<OpenWeatherResponse>(
            cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("OpenWeather returned an empty response.");

        var skyCondition = payload.Conditions.FirstOrDefault()?.Description
            ?? throw new InvalidOperationException("OpenWeather response has no sky condition.");

        return new WeatherObservation(
            payload.City,
            payload.System.Country,
            DateTimeOffset.FromUnixTimeSeconds(payload.Timestamp),
            payload.Wind.Speed,
            payload.Wind.DirectionDegrees,
            payload.VisibilityMeters,
            payload.Main.PressureHpa,
            skyCondition,
            payload.Main.TemperatureFahrenheit,
            CalculateDewPointFahrenheit(
                payload.Main.TemperatureFahrenheit,
                payload.Main.RelativeHumidityPercent),
            payload.Main.RelativeHumidityPercent);
    }

    private static double CalculateDewPointFahrenheit(
        double temperatureFahrenheit,
        int relativeHumidityPercent)
    {
        var temperatureCelsius = TemperatureConverter.ConvertFahrenheitToCelsius(
            temperatureFahrenheit);
        var relativeHumidity = Math.Clamp(relativeHumidityPercent, 1, 100) / 100.0;

        // OpenWeather's current-weather payload omits dew point; derive it with the Magnus approximation.
        var gamma = Math.Log(relativeHumidity) +
            (17.625 * temperatureCelsius) / (243.04 + temperatureCelsius);
        var dewPointCelsius = (243.04 * gamma) / (17.625 - gamma);

        return (dewPointCelsius * 9 / 5) + 32;
    }

    private sealed class OpenWeatherResponse
    {
        [JsonPropertyName("name")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("sys")]
        public OpenWeatherSystem System { get; set; } = new();

        [JsonPropertyName("dt")]
        public long Timestamp { get; set; }

        [JsonPropertyName("wind")]
        public OpenWeatherWind Wind { get; set; } = new();

        [JsonPropertyName("visibility")]
        public int VisibilityMeters { get; set; }

        [JsonPropertyName("main")]
        public OpenWeatherMain Main { get; set; } = new();

        [JsonPropertyName("weather")]
        public List<OpenWeatherCondition> Conditions { get; set; } = [];
    }

    private sealed class OpenWeatherSystem
    {
        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;
    }

    private sealed class OpenWeatherWind
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        [JsonPropertyName("deg")]
        public double DirectionDegrees { get; set; }
    }

    private sealed class OpenWeatherMain
    {
        [JsonPropertyName("temp")]
        public double TemperatureFahrenheit { get; set; }

        [JsonPropertyName("pressure")]
        public double PressureHpa { get; set; }

        [JsonPropertyName("humidity")]
        public int RelativeHumidityPercent { get; set; }
    }

    private sealed class OpenWeatherCondition
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}
namespace Weatheria.Application.Features.Weather;

public sealed record WeatherDto(
    string City,
    string Country,
    DateTimeOffset TimeUtc,
    double WindSpeedMph,
    double WindDirectionDegrees,
    int VisibilityMeters,
    double PressureHpa,
    string SkyCondition,
    double TemperatureFahrenheit,
    double TemperatureCelsius,
    double DewPointFahrenheit,
    int RelativeHumidityPercent);
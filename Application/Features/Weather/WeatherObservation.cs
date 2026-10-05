namespace Weatheria.Application.Features.Weather;

public sealed record WeatherObservation(
    string City,
    string Country,
    DateTimeOffset TimeUtc,
    double WindSpeedMph,
    double WindDirectionDegrees,
    int VisibilityMeters,
    double PressureHpa,
    string SkyCondition,
    double TemperatureFahrenheit,
    double DewPointFahrenheit,
    int RelativeHumidityPercent);
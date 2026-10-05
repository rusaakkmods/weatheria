using MediatR;
using Weatheria.Application.Abstractions;
using Weatheria.Domain.Services;

namespace Weatheria.Application.Features.Weather;

public sealed class GetWeatherQueryHandler(IWeatherService weatherService)
    : IRequestHandler<GetWeatherQuery, WeatherDto>
{
    public async Task<WeatherDto> Handle(
        GetWeatherQuery request,
        CancellationToken cancellationToken)
    {
        var observation = await weatherService.GetWeatherAsync(
            request.CityName,
            cancellationToken);

        return new WeatherDto(
            observation.City,
            observation.Country,
            observation.TimeUtc,
            observation.WindSpeedMph,
            observation.WindDirectionDegrees,
            observation.VisibilityMeters,
            observation.PressureHpa,
            observation.SkyCondition,
            observation.TemperatureFahrenheit,
            TemperatureConverter.ConvertFahrenheitToCelsius(
                observation.TemperatureFahrenheit),
            observation.DewPointFahrenheit,
            observation.RelativeHumidityPercent);
    }
}
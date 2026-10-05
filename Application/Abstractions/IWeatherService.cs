using Weatheria.Application.Features.Weather;

namespace Weatheria.Application.Abstractions;

public interface IWeatherService
{
    Task<WeatherObservation> GetWeatherAsync(
        string cityName,
        CancellationToken cancellationToken);
}
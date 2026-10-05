using MediatR;

namespace Weatheria.Application.Features.Weather;

public sealed record GetWeatherQuery(string CityName) : IRequest<WeatherDto>;
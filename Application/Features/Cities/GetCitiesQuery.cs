using MediatR;

namespace Weatheria.Application.Features.Cities;

public sealed record GetCitiesQuery(string CountryCode) : IRequest<IReadOnlyList<CityDto>>;
using MediatR;

namespace Weatheria.Application.Features.Countries;

public sealed record GetCountriesQuery : IRequest<IReadOnlyList<CountryDto>>;
using MediatR;
using Weatheria.Application.Abstractions;

namespace Weatheria.Application.Features.Cities;

public sealed class GetCitiesQueryHandler(ICountryCatalog catalog)
    : IRequestHandler<GetCitiesQuery, IReadOnlyList<CityDto>>
{
    public Task<IReadOnlyList<CityDto>> Handle(
        GetCitiesQuery request,
        CancellationToken cancellationToken)
    {
        return catalog.GetCitiesByCountryCodeAsync(
            request.CountryCode,
            cancellationToken);
    }
}
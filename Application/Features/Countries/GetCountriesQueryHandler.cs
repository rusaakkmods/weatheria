using MediatR;
using Weatheria.Application.Abstractions;

namespace Weatheria.Application.Features.Countries;

public sealed class GetCountriesQueryHandler(ICountryCatalog catalog) : IRequestHandler<GetCountriesQuery, IReadOnlyList<CountryDto>>
{
    public async Task<IReadOnlyList<CountryDto>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        return await catalog.GetCountriesAsync(cancellationToken);
    }
}
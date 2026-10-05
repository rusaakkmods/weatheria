using Microsoft.IdentityModel.Tokens;
using Weatheria.Application.Abstractions;
using Weatheria.Application.Features.Countries;

namespace Weatheria.Infrastructure.Catalog;

public sealed class CountryCatalog : ICountryCatalog
{
    private static readonly IReadOnlyList<CountryDto> Countries = new List<CountryDto>
    {
        new("Indonesia", "ID"),
        new("Australia", "AU"),
        new("Japan", "JP"),
        new("China", "CN")
    };

    public Task<IReadOnlyList<CountryDto>> GetCountriesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Countries);
    }
}
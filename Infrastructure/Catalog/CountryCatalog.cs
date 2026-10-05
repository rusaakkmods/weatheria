using Microsoft.IdentityModel.Tokens;
using Weatheria.Application.Abstractions;
using Weatheria.Application.Features.Cities;
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

    private static readonly IReadOnlyDictionary<string, IReadOnlyList<CityDto>> CitiesByCountryCode =
        new Dictionary<string, IReadOnlyList<CityDto>>(StringComparer.OrdinalIgnoreCase)
        {
            ["ID"] = [new("Jakarta"), new("Bandung"), new("Surabaya")],
            ["AU"] = [new("Sydney"), new("Melbourne"), new("Brisbane")],
            ["JP"] = [new("Tokyo"), new("Osaka"), new("Kyoto")],
            ["CN"] = [new("Beijing"), new("Shanghai"), new("Guangzhou")]
        };

    public Task<IReadOnlyList<CountryDto>> GetCountriesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Countries);
    }

    public Task<IReadOnlyList<CityDto>> GetCitiesByCountryCodeAsync(
        string countryCode,
        CancellationToken cancellationToken)
    {
        CitiesByCountryCode.TryGetValue(countryCode, out var cities);
        return Task.FromResult(cities ?? Array.Empty<CityDto>());
    }
}
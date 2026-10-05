using Weatheria.Application.Features.Countries;
using Weatheria.Application.Features.Cities;

namespace Weatheria.Application.Abstractions;

public interface ICountryCatalog
{
    Task<IReadOnlyList<CountryDto>> GetCountriesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<CityDto>> GetCitiesByCountryCodeAsync(
        string countryCode,
        CancellationToken cancellationToken);
}
using Weatheria.Application.Features.Countries;

namespace Weatheria.Application.Abstractions;

public interface ICountryCatalog
{
    Task<IReadOnlyList<CountryDto>> GetCountriesAsync(CancellationToken cancellationToken);
}
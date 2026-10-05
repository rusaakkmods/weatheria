using MediatR;
using Microsoft.AspNetCore.Mvc;
using Weatheria.Application.Features.Cities;
using Weatheria.Application.Features.Countries;

namespace Weatheria.Api.Controllers;

[ApiController]
[Route("api/countries")]
public sealed class CountriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CountryDto>>> Get(
        CancellationToken cancellationToken)
    {
        var countries = await sender.Send(
            new GetCountriesQuery(),
            cancellationToken);

        return Ok(countries);
    }

    [HttpGet("{countryCode}/cities")]
    public async Task<ActionResult<IReadOnlyList<CityDto>>> GetCities(
        string countryCode,
        CancellationToken cancellationToken)
    {
        var cities = await sender.Send(
            new GetCitiesQuery(countryCode),
            cancellationToken);

        return Ok(cities);
    }
}
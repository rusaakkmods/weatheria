using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Weatheria.Application.Features.Countries;
using Xunit;

namespace Weatheria.Tests.Api.Countries;

public class GetCountriesEndpointTests
{
    [Fact]
    public async Task Get_ShouldReturnAvailableCountries()
    {
        await using var factory = new WeatheriaWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/countries");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var countries = await response.Content.ReadFromJsonAsync<List<CountryDto>>();

        countries.Should().NotBeNull();
        countries!.Should().BeEquivalentTo(
        [
            new CountryDto("Indonesia", "ID"),
            new CountryDto("Australia", "AU"),
            new CountryDto("Japan", "JP"),
            new CountryDto("China", "CN")
        ]);
    }
}
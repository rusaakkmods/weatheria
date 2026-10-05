using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Weatheria.Application.Features.Cities;
using Xunit;

namespace Weatheria.Tests.Api.Countries;

public class GetCitiesEndpointTests
{
    [Fact]
    public async Task Get_ShouldReturnCitiesForCountryCode()
    {
        await using var factory = new WeatheriaWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/countries/ID/cities");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cities = await response.Content.ReadFromJsonAsync<List<CityDto>>();

        cities.Should().NotBeNull();
        cities!.Should().BeEquivalentTo(
        [
            new CityDto("Jakarta"),
            new CityDto("Bandung"),
            new CityDto("Surabaya")
        ]);
    }
}
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace Weatheria.Tests.Api.WeatherNotes;

public class CreateWeatherNoteEndpointTests
{
    [Fact]
    public async Task Post_ShouldCreateWeatherNote()
    {
        await using var factory = new WeatheriaWebApplicationFactory();

        var client = factory.CreateClient();

        var request = new
        {
            city = "Bandung",
            content = "Hujan Sedikit"
        };

        var response = await client.PostAsJsonAsync("/api/weather/notes", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateWeatherNoteResponse>();

        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    private sealed record CreateWeatherNoteResponse(Guid Id);
}
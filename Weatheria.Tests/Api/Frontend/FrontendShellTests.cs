using System.Net;
using FluentAssertions;
using Xunit;

namespace Weatheria.Tests.Api.Frontend;

public class FrontendShellTests
{
    [Fact]
    public async Task Root_ShouldServeFrontendAndItsAssets()
    {
        await using var factory = new WeatheriaWebApplicationFactory();
        var client = factory.CreateClient();

        var pageResponse = await client.GetAsync("/");

        pageResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        pageResponse.Content.Headers.ContentType?.MediaType.Should().Be("text/html");
        var page = await pageResponse.Content.ReadAsStringAsync();
        page.Should().Contain("Weatheria");
        page.Should().Contain("for=\"country-select\">Country</label>");
        page.Should().Contain("<option value=\"\">Select a country</option>");
        page.Should().Contain("for=\"city-select\">City</label>");
        page.Should().Contain("<option value=\"\">Select a country first</option>");
        page.Should().Contain("id=\"weather-panel\" aria-labelledby=\"weather-title\" hidden");
        foreach (var label in new[]
        {
            "Location",
            "Time (UTC)",
            "Temperature (°F)",
            "Temperature (°C)",
            "Dew Point (°F)",
            "Relative Humidity",
            "Wind Speed",
            "Wind Direction",
            "Visibility",
            "Pressure",
            "Sky Condition"
        })
        {
            page.Should().Contain($"<dt>{label}</dt>");
        }
        page.Should().Contain("id=\"weather-note-form\"");
        page.Should().Contain("for=\"weather-note\">Weather note</label>");
        page.Should().Contain("id=\"weather-note\" name=\"content\"");
        page.Should().Contain("id=\"save-note\" type=\"submit\"");
        page.Should().Contain("id=\"note-status\"");
        page.Should().Contain("/styles.css");
        page.Should().Contain("/app.js");

        var stylesheetResponse = await client.GetAsync("/styles.css");
        stylesheetResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var scriptResponse = await client.GetAsync("/app.js");
        scriptResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
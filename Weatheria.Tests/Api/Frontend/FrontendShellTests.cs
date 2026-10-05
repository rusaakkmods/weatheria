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
        page.Should().Contain("/styles.css");
        page.Should().Contain("/app.js");

        var stylesheetResponse = await client.GetAsync("/styles.css");
        stylesheetResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var scriptResponse = await client.GetAsync("/app.js");
        scriptResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
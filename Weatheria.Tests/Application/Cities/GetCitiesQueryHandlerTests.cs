using FluentAssertions;
using Moq;
using Weatheria.Application.Abstractions;
using Weatheria.Application.Features.Cities;
using Xunit;

namespace Weatheria.Tests.Application.Cities;

public class GetCitiesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnCitiesForRequestedCountryCode()
    {
        var catalog = new Mock<ICountryCatalog>();
        var expectedCities = new[]
        {
            new CityDto("Jakarta"),
            new CityDto("Bandung")
        };

        catalog
            .Setup(x => x.GetCitiesByCountryCodeAsync("ID", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCities);

        var handler = new GetCitiesQueryHandler(catalog.Object);

        var result = await handler.Handle(
            new GetCitiesQuery("ID"),
            CancellationToken.None);

        result.Should().BeEquivalentTo(expectedCities);
        catalog.Verify(
            x => x.GetCitiesByCountryCodeAsync("ID", It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
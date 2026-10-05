using FluentAssertions;
using Moq;
using Xunit;
using Weatheria.Application.Abstractions;
using Weatheria.Application.Features.Countries;

namespace Weatheria.Tests.Application.Countries;

public class GetCountriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnListOfCountries()
    {
        // Arrange
        var catalog = new Mock<ICountryCatalog>();

        catalog
          .Setup(x => x.GetCountriesAsync(It.IsAny<CancellationToken>()))
          .ReturnsAsync(
            [
              new CountryDto("Indonesia", "ID"),
              new CountryDto("Australia", "AU")
            ]);

        var handler = new GetCountriesQueryHandler(catalog.Object);

        // Act
        var result = await handler.Handle(
            new GetCountriesQuery(),
            CancellationToken.None
        );

        // Assert
        result.Should().HaveCount(2);

        result.Should().Contain(c => c.Name == "Indonesia" && c.Code == "ID");
    }
}
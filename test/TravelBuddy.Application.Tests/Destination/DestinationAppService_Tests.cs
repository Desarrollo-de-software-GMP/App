using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using TravelBuddy;
using TravelBuddy.Application.Contracts.Destinations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;
using TravelBuddy.Destinations;


namespace TravelBuddy.Destinations;

public abstract class DestinationAppService_Tests<TStartupModule> : TravelBuddyApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IDestinationAppService _service;

    protected DestinationAppService_Tests()
    {
        _service = GetRequiredService<IDestinationAppService>();
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedDestinationDto()
    {
        //Arrange
        var input = new CreateUpdateDestinationDTO
        {
            Name = "Concepcion del Uruguay",
            Country = "Argentina",
            Poblation = 150000,
            PhotoUrl = "http://example.com/photo.jpg",
            LastUpdate = DateTime.UtcNow,
            Coordinates = new CoordinatesDTO { Latitude = 407128, Longitude = -740060 }

        };
        //Act
        var result = await _service.CreateAsync(input);
        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe(input.Name);
        result.Country.ShouldBe(input.Country);
        result.Poblation.ShouldBe(input.Poblation);
        result.PhotoUrl.ShouldBe(input.PhotoUrl);
        result.LastUpdate.ShouldBe(input.LastUpdate);
        result.Coordinates.Latitude.ShouldBe(input.Coordinates.Latitude);
        result.Coordinates.Longitude.ShouldBe(input.Coordinates.Longitude);

    }

    [Fact]
    public async Task CreateAsync_ShouldPersistDestinationInDatabase()
    {
        //Arrange
        var input = new CreateUpdateDestinationDTO
        {
            Name = "Concepcion del Uruguay",
            Country = "Argentina",
            Poblation = 150000,
            PhotoUrl = "http://example.com/photo.jpg",
            LastUpdate = DateTime.UtcNow,
            Coordinates = new CoordinatesDTO { Latitude = 407128, Longitude = -740060 }

        };
        //Act
        var createdDestination = await _service.CreateAsync(input);
        var retrievedDestination = await _service.GetAsync(createdDestination.Id);

        //Assert
        retrievedDestination.ShouldNotBeNull();
        retrievedDestination.Name.ShouldBe(input.Name);
        retrievedDestination.Country.ShouldBe(input.Country);
        retrievedDestination.Poblation.ShouldBe(input.Poblation);
        retrievedDestination.PhotoUrl.ShouldBe(input.PhotoUrl);
        retrievedDestination.LastUpdate.ShouldBe(input.LastUpdate);
        retrievedDestination.Coordinates.Latitude.ShouldBe(input.Coordinates.Latitude);
        retrievedDestination.Coordinates.Longitude.ShouldBe(input.Coordinates.Longitude);

    }

    [Fact]
    public async Task CreateAsync_ShouldThrowExceptionWhenCountryIsNull()
    {
        //Arrange
        var input = new CreateUpdateDestinationDTO
        {
            Name = "Concepcion del Uruguay",
            Country = null!, // Country is required
            Poblation = 150000,
            PhotoUrl = "http://example.com/photo.jpg",
            LastUpdate = DateTime.UtcNow,
            Coordinates = new CoordinatesDTO { Latitude = 407128, Longitude = -740060 }
        };
        //Act & Assert
        await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _service.CreateAsync(input);
        });
    }
}   



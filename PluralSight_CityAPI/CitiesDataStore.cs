using PluralSight_CityAPI.Models;

namespace PluralSight_CityAPI
{
    public class CitiesDataStore
    {
        public List<CityDto> Cities { get; set; }
        public static CitiesDataStore Current { get; } = new CitiesDataStore();

        public CitiesDataStore() {
            Cities = new List<CityDto> { 
                    new CityDto()
                    {
                        Id = 1,
                        Name = "HCM",
                        Description = "Hot",
                        PointsOfInterest = new List<PointOfInterestDto>()
                        {
                            new PointOfInterestDto {
                                Id = 1, Name = "Ben Thanh", Description = "Pricey"
                            }
                        }
                    },
                    new CityDto()
                    {
                        Id = 2,
                        Name = "Melbourne",
                        Description = "Cold",
                        PointsOfInterest = new List<PointOfInterestDto>()
                        {
                            new PointOfInterestDto
                            {
                                Id=2, Name = "Park", Description = "Green"
                            }
                        }
                    },
                    new CityDto()
                    {
                        Id = 3,
                        Name = "London",
                        Description = "No Clue",
                        PointsOfInterest = new List<PointOfInterestDto>
                        {
                            new PointOfInterestDto
                            {
                                Id=3, Name = "Big Ben", Description = "Clock" 
                            }
                        }
                    }
             };    
        }

    }
}

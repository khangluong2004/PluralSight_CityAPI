using AutoMapper;

namespace PluralSight_CityAPI.Profiles
{
    public class CityProfile: Profile
    {
        public CityProfile() {
            CreateMap<Entities.City, Models.CityWithoutPointDto>();
            CreateMap<Entities.City, Models.CityDto>();
        }
    }
}

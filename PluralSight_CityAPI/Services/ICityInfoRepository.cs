using PluralSight_CityAPI.Entities;

namespace PluralSight_CityAPI.Services
{
    public interface ICityInfoRepository
    {
        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
        Task<City?> GetCityAsync(int cityId, bool includePoints);

        Task<bool> CityNameMatchId(string cityName, int cityId);

        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestsAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointId);
        Task AddPointOfInterestAsync(int cityId, PointOfInterest point);

        void DeletePointOfInterestAsync(PointOfInterest pointOfInterest);
        Task<bool> SaveChangesAsync();
    }
}

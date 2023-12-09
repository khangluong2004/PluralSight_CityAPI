using Microsoft.EntityFrameworkCore;
using PluralSight_CityAPI.DbContexts;
using PluralSight_CityAPI.Entities;

namespace PluralSight_CityAPI.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {   
            // Convert DbSet to IQueryable
            var query = _context.Cities as IQueryable<City>;

            // Query name
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                query = query.Where(c => c.Name == name);
            }

            // Query search query
            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                query = query.Where(c => (c.Name.Contains(searchQuery) || (c.Description != null && c.Description.Contains(searchQuery))));
            }

            // Get total item
            var totalItem = _context.Cities.Count();

            // Pagination item
            var pageDetails = new PaginationMetadata(totalItem, pageSize, pageNumber);

            return (await query
                    .OrderBy(c => c.Name)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync(), pageDetails);
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePoints)
        {
            if (includePoints)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefaultAsync();
            } else
            {
                return await _context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }
        }

        public async Task<bool> CityNameMatchId(string cityName, int cityId)
        {
            return (await _context.Cities.FirstOrDefaultAsync(c => c.Id == cityId && c.Name == cityName) != null);
        }

        public async Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointId)
        {
            return await _context.PointOfInterests.Where(p => p.CityId == cityId && p.Id == pointId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestsAsync(int cityId)
        {
            return await _context.PointOfInterests.Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task AddPointOfInterestAsync(int cityId, PointOfInterest point)
        {
            var cityFound = await GetCityAsync(cityId, true);
            if (cityFound != null)
            {
                cityFound.PointsOfInterest.Add(point);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public void DeletePointOfInterestAsync(PointOfInterest pointOfInterest)
        {
            _context.PointOfInterests.Remove(pointOfInterest);
        }
    }
}

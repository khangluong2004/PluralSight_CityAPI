using Microsoft.EntityFrameworkCore;
using PluralSight_CityAPI.Entities;

namespace PluralSight_CityAPI.DbContexts
{
    public class CityInfoContext: DbContext
    {
        // Non-null is guaranteed by convention from DbContext
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<PointOfInterest> PointOfInterests { get; set; } = null!;

        // Configure DbContext options to connect to the Db
        // The DbContext constructor has an overload with "options" as a parameter
        public CityInfoContext(DbContextOptions<CityInfoContext> options): base(options) { }

        // Override OnModelCreating to seed database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                new City("HCM")
                {
                    Id = 1,
                    Description = "Hot"
                },
                new City("Melbourne")
                {
                    Id=2,
                    Description = "Cold"
                },
                new City("London")
                {
                    Id=3,
                    Description = "No Clue"
                }
                );
            modelBuilder.Entity<PointOfInterest>().HasData(
                new PointOfInterest("Ben Thanh")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "Pricey"
                },
                new PointOfInterest("Library") { 
                    Id = 2,
                    CityId = 2,
                    Description = "Books"
                },
                new PointOfInterest("Big Ben")
                {
                    Id = 3,
                    CityId = 3,
                    Description = "Clock"
                }
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}

namespace PluralSight_CityAPI.Models
{
    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<PointOfInterestDto> PointsOfInterest { get; set; } = new List<PointOfInterestDto>();
        public int NumberPoints { 
            get
            {
                return PointsOfInterest.Count;
            }
        }

        // No constructor to allow xml serializer

        //public CityDto(int id, string name, string descript) {
        //    Id = id;
        //    Name = name;
        //    Description = descript;
        //}
    }
}

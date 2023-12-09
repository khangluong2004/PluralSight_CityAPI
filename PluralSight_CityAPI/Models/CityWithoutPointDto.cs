namespace PluralSight_CityAPI.Models
{
    public class CityWithoutPointDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}

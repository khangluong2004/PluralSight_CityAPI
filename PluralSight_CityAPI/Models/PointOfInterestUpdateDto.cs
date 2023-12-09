using System.ComponentModel.DataAnnotations;

namespace PluralSight_CityAPI.Models
{
    public class PointOfInterestUpdateDto
    {
        [Required(ErrorMessage = "Name?")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }
    }
}

using PluralSight_CityAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PluralSight_CityAPI.Entities
{
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        // Identity = Generate at insertion only
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<PointOfInterest> PointsOfInterest { get; set; } = new List<PointOfInterest>();
        public City(string name)
        {
            Name = name;
        }
    }
}

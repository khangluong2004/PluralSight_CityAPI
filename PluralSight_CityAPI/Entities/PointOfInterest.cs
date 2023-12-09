using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PluralSight_CityAPI.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        // This group of 3 lines forming a 1-to-many relationship
        // with City. Only 2nd line is required, other 2 added for clarity.
        [ForeignKey("CityId")]
        public City? City { get; set; }
        public int CityId { get; set; }

        public PointOfInterest(string name)
        {
            Name = name;
        }
    }
}

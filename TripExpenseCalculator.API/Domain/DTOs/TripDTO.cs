using System.ComponentModel.DataAnnotations;

namespace TripExpenseCalculator.API.Domain.DTOs
{
    public class TripDTO
    {
        public Guid ID { get; set; }
        [Required]
        public string Name { get; set; }
        public List<TravelerDTO> Travelers { get; set; } = new List<TravelerDTO>();
    }
}

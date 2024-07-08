using System.ComponentModel.DataAnnotations;

namespace TripExpenseCalculator.API.Domain.DTOs
{
    public class TravelerDTO
    {
        public Guid ID { get; set; }
        [Required]
        public string Name { get; set; }
        public List<ExpenseDTO> Expenses { get; set; } = new List<ExpenseDTO>();
    }
}

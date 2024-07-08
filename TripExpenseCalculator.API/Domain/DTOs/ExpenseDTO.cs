using System.ComponentModel.DataAnnotations;

namespace TripExpenseCalculator.API.Domain.DTOs
{
    public class ExpenseDTO
    {
        public Guid ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Cost { get; set; }
    }
}

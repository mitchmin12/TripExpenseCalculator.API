namespace TripExpenseCalculator.API.Persistence.Models
{
    public class Expense
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }

        public Guid TravelerID { get; set; }
        public Traveler Traveler { get; set; }
    }
}

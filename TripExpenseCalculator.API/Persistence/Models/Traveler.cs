namespace TripExpenseCalculator.API.Persistence.Models
{
    public class Traveler
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public List<Expense> Expenses { get; set; } = new List<Expense>();

        public Guid TripID { get; set; }
        public Trip Trip { get; set; }
    }
}

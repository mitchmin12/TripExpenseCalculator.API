namespace TripExpenseCalculator.API.Persistence.Models
{
    public class Trip
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public List<Traveler> Travelers { get; set; } = new List<Traveler>();
    }
}

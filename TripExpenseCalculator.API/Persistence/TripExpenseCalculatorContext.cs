using Microsoft.EntityFrameworkCore;
using TripExpenseCalculator.API.Persistence.Models;

namespace TripExpenseCalculator.API.Persistence
{
    public class TripExpenseCalculatorContext : DbContext
    {
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Traveler> Travelers { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        public TripExpenseCalculatorContext(DbContextOptions<TripExpenseCalculatorContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Trip>().ToTable("Trips");
            modelBuilder.Entity<Trip>().HasKey(trip => trip.ID);
            modelBuilder.Entity<Trip>().Property(trip => trip.ID).IsRequired().ValueGeneratedOnAdd();
            modelBuilder.Entity<Trip>().Property(trip => trip.Name).IsRequired();
            modelBuilder.Entity<Trip>().HasMany(trip => trip.Travelers).WithOne(traveler => traveler.Trip).HasForeignKey(traveler => traveler.TripID).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Traveler>().ToTable("Travelers");
            modelBuilder.Entity<Traveler>().HasKey(traveler => traveler.ID);
            modelBuilder.Entity<Traveler>().Property(traveler => traveler.ID).IsRequired().ValueGeneratedOnAdd();
            modelBuilder.Entity<Traveler>().Property(traveler => traveler.Name).IsRequired();
            modelBuilder.Entity<Traveler>().HasMany(traveler => traveler.Expenses).WithOne(expense => expense.Traveler).HasForeignKey(expense => expense.TravelerID).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Expense>().ToTable("Expenses");
            modelBuilder.Entity<Expense>().HasKey(expense => expense.ID);
            modelBuilder.Entity<Expense>().Property(expense => expense.ID).IsRequired().ValueGeneratedOnAdd();
            modelBuilder.Entity<Expense>().Property(expense => expense.Name).IsRequired();
            modelBuilder.Entity<Expense>().Property(expense => expense.Cost).IsRequired();

            var trip1 = new Trip
            {
                ID = MagicStrings.FunTripId,
                Name = "A trip to the store"
            };

            var trip2 = new Trip
            {
                ID = MagicStrings.TerribleTripId,
                Name = "An Incomplete Trip"
            };

            modelBuilder.Entity<Trip>().HasData
            (
                trip1,
                trip2
            );

            var traveler1 = new Traveler
            {
                ID = Guid.NewGuid(),
                Name = "Mitch",
                TripID = trip1.ID,
            };

            var traveler2 = new Traveler
            {
                ID = Guid.NewGuid(),
                Name = "Nate",
                TripID = trip1.ID,
            };

            modelBuilder.Entity<Traveler>().HasData
            (
                traveler1,
                traveler2
            );

            var mitchExpense1 = new Expense
            {
                ID = Guid.NewGuid(),
                Name = "Food",
                Cost = 125.50m,
                TravelerID = traveler1.ID
            };
            var mitchExpense2 = new Expense
            {
                ID = Guid.NewGuid(),
                Name = "Gas",
                Cost = 15m,
                TravelerID = traveler1.ID
            };
            var nateExpense1 = new Expense
            {
                ID = Guid.NewGuid(),
                Name = "Beer",
                Cost = 250m,
                TravelerID = traveler2.ID
            };

            modelBuilder.Entity<Expense>().HasData
            (
                mitchExpense1,
                mitchExpense2,
                nateExpense1
            );
        }
    }
}

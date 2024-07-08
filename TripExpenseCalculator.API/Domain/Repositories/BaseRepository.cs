using TripExpenseCalculator.API.Persistence;

namespace TripExpenseCalculator.API.Domain.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly TripExpenseCalculatorContext _context;

        public BaseRepository(TripExpenseCalculatorContext context)
        {
            _context = context;
        }
    }
}

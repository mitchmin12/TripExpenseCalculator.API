using TripExpenseCalculator.API.Persistence;

namespace TripExpenseCalculator.API.Domain.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TripExpenseCalculatorContext _context;

        public UnitOfWork(TripExpenseCalculatorContext context)
        {
            _context = context;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

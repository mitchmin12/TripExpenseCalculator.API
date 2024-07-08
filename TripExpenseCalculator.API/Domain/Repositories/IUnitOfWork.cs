namespace TripExpenseCalculator.API.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task SaveAsync();
    }
}

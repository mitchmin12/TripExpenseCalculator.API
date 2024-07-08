using TripExpenseCalculator.API.Domain.DTOs;
using TripExpenseCalculator.API.Domain.Responses;

namespace TripExpenseCalculator.API.Domain.Repositories
{
    public interface ITripRepository
    {
        Task<Response<TripDTO>> GetTripById(Guid id);
        Task<Response<IEnumerable<TripDTO>>> GetAllTripsAsync();
        Task<Response<TripDTO>> AddTripAsync(TripDTO tripDTO);
        Response UpdateTrip(Guid id, TripDTO tripDTO);
        Response<TripDTO> DeleteTrip(Guid id);
    }
}

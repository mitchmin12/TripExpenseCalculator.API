using TripExpenseCalculator.API.Domain.DTOs;
using TripExpenseCalculator.API.Domain.Responses;

namespace TripExpenseCalculator.API.Domain.Services
{
    public interface ITripService
    {
        Task<Response<TripDTO>> GetTripById(Guid id);
        Task<Response<IEnumerable<TripDTO>>> GetAllTrips();
        Task<Response<TripDTO>> SaveTripAsync(TripDTO tripDTO);
        Task<Response<TripDTO>> UpdateTripAsync(Guid id, TripDTO tripDTO);
        Task<Response<TripDTO>> DeleteTripAsync(Guid id);
    }
}

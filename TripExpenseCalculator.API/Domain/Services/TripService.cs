using TripExpenseCalculator.API.Domain.DTOs;
using TripExpenseCalculator.API.Domain.Repositories;
using TripExpenseCalculator.API.Domain.Responses;

namespace TripExpenseCalculator.API.Domain.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TripService(ITripRepository tripRepository, IUnitOfWork unitOfWork)
        {
            _tripRepository = tripRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Response<TripDTO>> GetTripById(Guid id)
        {
            return await _tripRepository.GetTripById(id);
        }

        public async Task<Response<IEnumerable<TripDTO>>> GetAllTrips()
        {
            return await _tripRepository.GetAllTripsAsync();
        }


        public async Task<Response<TripDTO>> SaveTripAsync(TripDTO tripDTO)
        {
            try
            {
                var response = await _tripRepository.AddTripAsync(tripDTO);
                await _unitOfWork.SaveAsync();

                return response;
            }
            catch (Exception e)
            {
                return Response.Fail<TripDTO>(e.Message);
            }
        }

        public async Task<Response<TripDTO>> UpdateTripAsync(Guid id, TripDTO tripDTO)
        {
            try
            {
                var response = _tripRepository.UpdateTrip(id, tripDTO);
                await _unitOfWork.SaveAsync();

                return await GetTripById(id);
            }
            catch (Exception e)
            {
                return Response.Fail<TripDTO>(e.Message);
            }
        }

        public async Task<Response<TripDTO>> DeleteTripAsync(Guid id)
        {
            try
            {
                var response = _tripRepository.DeleteTrip(id);
                await _unitOfWork.SaveAsync();

                return response;
            }
            catch (Exception e)
            {
                return Response.Fail<TripDTO>(e.Message);
            }
        }
    }
}

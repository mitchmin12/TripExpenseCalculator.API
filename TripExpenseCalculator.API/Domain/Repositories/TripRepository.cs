using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripExpenseCalculator.API.Domain.DTOs;
using TripExpenseCalculator.API.Domain.Responses;
using TripExpenseCalculator.API.Persistence;
using TripExpenseCalculator.API.Persistence.Models;

namespace TripExpenseCalculator.API.Domain.Repositories
{
    public class TripRepository : BaseRepository, ITripRepository
    {
        private readonly IMapper _mapper;

        public TripRepository(IMapper mapper, TripExpenseCalculatorContext context) : base(context)
        {
            _mapper = mapper;
        }
        public async Task<Response<TripDTO>> GetTripById(Guid id)
        {
            try
            {
                var trip = await _context.Trips.Include(trip => trip.Travelers)
                                               .ThenInclude(traveler => traveler.Expenses)
                                               .FirstOrDefaultAsync(trip => trip.ID == id);

                var dto = _mapper.Map<TripDTO>(trip);

                return Response.OK(dto);
            }
            catch (Exception ex)
            {
                return Response.Fail<TripDTO>(ex.Message);
            }
        }

        public async Task<Response<IEnumerable<TripDTO>>> GetAllTripsAsync()
        {
            try
            {
                var trips = await _context.Trips.Include(trip => trip.Travelers)
                                                .ThenInclude(traveler => traveler.Expenses)
                                                .ToListAsync();

                var dtos = trips.Select(_mapper.Map<Trip, TripDTO>);

                return Response.OK(dtos);
            }

            catch (Exception ex)
            {
                return Response.Fail<IEnumerable<TripDTO>>(ex.Message);
            }
        }
        public async Task<Response<TripDTO>> AddTripAsync(TripDTO tripDTO)
        {
            try
            {
                var trip = _mapper.Map<Trip>(tripDTO);

                var value = await _context.Trips.AddAsync(trip);

                var dtos = _mapper.Map<TripDTO>(value.Entity);

                return Response.OK(dtos);
            }
            catch (Exception ex)
            {
                return Response.Fail<TripDTO>(ex.Message);
            }
        }

        public Response UpdateTrip(Guid id, TripDTO tripDTO)
        {
            try
            {
                var existingTrip = _context.Trips.Include(trip => trip.Travelers)
                                                 .ThenInclude(traveler => traveler.Expenses)
                                                 .First(trip => trip.ID == id);

                existingTrip.Name = tripDTO.Name;

                foreach (var existingTraveler in existingTrip.Travelers)
                {
                    if (!tripDTO.Travelers.Any(traveler => traveler.ID == existingTraveler.ID))
                    {
                        _context.Travelers.Remove(existingTraveler);
                    }
                }
                foreach (var traveler in tripDTO.Travelers)
                {
                    var existingTraveler = existingTrip.Travelers.Where(t => t.ID == traveler.ID).SingleOrDefault();

                    if (existingTraveler != null)
                    {
                        existingTraveler.Name = traveler.Name;

                        foreach (var existingExpense in existingTraveler.Expenses)
                        {
                            if (!traveler.Expenses.Any(expense => expense.ID == existingExpense.ID))
                            {
                                _context.Expenses.Remove(existingExpense);
                            }
                        }

                        foreach (var expense in traveler.Expenses)
                        {
                            var existingExpense = existingTraveler.Expenses.Where(e => e.ID == expense.ID).SingleOrDefault();

                            if (existingExpense != null)
                            {
                                existingExpense.Cost = expense.Cost;
                                existingExpense.Name = expense.Name;
                            }
                            else
                            {
                                var newExpense = _mapper.Map<Expense>(expense);
                                newExpense.Traveler = existingTraveler;
                                _context.Expenses.Add(newExpense);
                            }
                        }
                    }
                    else
                    {
                        var newTraveler = _mapper.Map<Traveler>(traveler);
                        newTraveler.Trip = existingTrip;
                        _context.Travelers.Add(newTraveler);
                    }
                }

                return Response.OK(_mapper.Map<TripDTO>(existingTrip));
            }
            catch (Exception e)
            {
                return Response.Fail<TripDTO>(e.Message);
            }

        }

        public Response<TripDTO> DeleteTrip(Guid id)
        {
            try
            {
                var trip = _context.Trips.Where(trip => trip.ID == id).FirstOrDefault();
                _context.Trips.Remove(trip);

                var dto = _mapper.Map<TripDTO>(trip);

                return Response.OK(dto);
            }
            catch (Exception ex)
            {
                return Response.Fail<TripDTO>(ex.Message);
            }
        }
    }
}

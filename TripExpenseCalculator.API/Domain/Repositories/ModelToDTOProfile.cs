using AutoMapper;
using TripExpenseCalculator.API.Domain.DTOs;
using TripExpenseCalculator.API.Persistence.Models;

namespace TripExpenseCalculator.API.Domain.Repositories
{
    public class ModelToDTOProfile : Profile
    {
        public ModelToDTOProfile()
        {
            CreateMap<Expense, ExpenseDTO>();
            CreateMap<Traveler, TravelerDTO>();
            CreateMap<Trip, TripDTO>();

            CreateMap<ExpenseDTO, Expense>();
            CreateMap<TravelerDTO, Traveler>();
            CreateMap<TripDTO, Trip>();
        }
    }
}

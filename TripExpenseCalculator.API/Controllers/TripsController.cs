using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TripExpenseCalculator.API.Domain.DTOs;
using TripExpenseCalculator.API.Domain.Services;

namespace TripExpenseCalculator.API.Controllers
{
    [Route("/api/[controller]")]
    public class TripsController : Controller
    {
        private readonly ITripService _tripService;

        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDTO>>> GetAllTripsAsync()
        {
            var response = await _tripService.GetAllTrips();

            if(response.IsFailure)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Value);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TripDTO>> GetTripAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorsHelper(ModelState));
            }

            var response = await _tripService.GetTripById(id);

            if(response.IsFailure)
            {
                return BadRequest(response.Message);
            }

            if(response.Value == null)
            {
                return NotFound();
            }

            return Ok(response.Value);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] TripDTO trip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorsHelper(ModelState));
            }

            var response = await _tripService.SaveTripAsync(trip);

            if (response.IsFailure)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] TripDTO trip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorsHelper(ModelState));
            }
            if (id != trip.ID)
            {
                return BadRequest("Trip with ID {" + id + "} doesn't match the id {" + trip.ID + "} of the data sent");
            }

            var response = await _tripService.UpdateTripAsync(id, trip);

            if (response.IsFailure)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var response = await _tripService.DeleteTripAsync(id);

            if(response.Value == null)
            {
                return NotFound();
            }

            if (response.IsFailure)
            {
                return BadRequest(response.Message);
            }

            return Ok();
        }

        #region Helpers
        private List<string> ErrorsHelper(ModelStateDictionary modelState)
        {
            return modelState.SelectMany(result => result.Value.Errors)
                             .Select(error => error.ErrorMessage)
                             .ToList();
        }
        #endregion
    }
}

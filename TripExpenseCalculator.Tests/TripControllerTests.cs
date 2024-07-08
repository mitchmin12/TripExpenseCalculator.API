using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using TripExpenseCalculator.API.Domain.DTOs;

namespace TripExpenseCalculator.Tests;

public class TripControllerTests
{
    //These tests are by no means comprehensive and there are a lot of other cases that
    //still need tested. But due to the time constraints I wasn't able to add others.
    //Deletes returning 404's, Puts validating that if the trip id from the route doesn't match
    //the trip id from the object, various other failure paths from the service/repo level,
    //etc. should all be tested as well. (service/repo level tests might be better suited as
    //actual unit tests instead of integration tests)

    [Fact]
    public async Task GetAllTripsAsync_ReturnsAllTrips()
    {
        // Arrange
        var app = new TripExpenseCalculatorWebApplicationFactory();
        var client = app.CreateClient();

        // Act
        var response = await client.GetAsync("/api/trips");

        // Assert
        response.EnsureSuccessStatusCode();
        var trips = await response.Content.ReadFromJsonAsync<IEnumerable<TripDTO>>();

        trips.Should().HaveCount(2);

        var funTrip = trips.First(trip => trip.ID == MagicStrings.FunTripId);
        funTrip.ID.Should().Be(MagicStrings.FunTripId);
        funTrip.Name.Should().Be("A trip to the store");
        funTrip.Travelers.Should().HaveCount(2);

        var mitch = funTrip.Travelers.First(traveler => traveler.Name == "Mitch");
        mitch.ID.Should().NotBe(Guid.Empty);
        mitch.Expenses.Should().HaveCount(2);

        var food = mitch.Expenses.First(expense => expense.Name == "Food");
        food.ID.Should().NotBe(Guid.Empty);
        food.Name.Should().Be("Food");
        food.Cost.Should().Be(125.5m);

        var gas = mitch.Expenses.First(expense => expense.Name == "Gas");
        gas.ID.Should().NotBe(Guid.Empty);
        gas.Name.Should().Be("Gas");
        gas.Cost.Should().Be(15m);

        var nate = funTrip.Travelers.First(traveler => traveler.Name == "Nate");
        nate.ID.Should().NotBe(Guid.Empty);
        nate.Expenses.Should().HaveCount(1);

        var beer = nate.Expenses.First();
        beer.ID.Should().NotBe(Guid.Empty);
        beer.Name.Should().Be("Beer");
        beer.Cost.Should().Be(250m);

        var terribleTrip = trips.First(trip => trip.ID == MagicStrings.TerribleTripId);
        terribleTrip.ID.Should().Be(MagicStrings.TerribleTripId);
        terribleTrip.Name.Should().Be("An Incomplete Trip");
        terribleTrip.Travelers.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetTripAsync_ReturnsTheRightTrip()
    {
        // Arrange
        var app = new TripExpenseCalculatorWebApplicationFactory();
        var client = app.CreateClient();

        // Act
        var response = await client.GetAsync("/api/trips/" + MagicStrings.FunTripId.ToString());

        // Assert
        response.EnsureSuccessStatusCode();
        var trip = await response.Content.ReadFromJsonAsync<TripDTO>();

        trip.ID.Should().Be(MagicStrings.FunTripId);
        trip.Name.Should().Be("A trip to the store");
        trip.Travelers.Should().HaveCount(2);

        var mitch = trip.Travelers.First(traveler => traveler.Name == "Mitch");
        mitch.ID.Should().NotBe(Guid.Empty);
        mitch.Expenses.Should().HaveCount(2);

        var food = mitch.Expenses.First(expense => expense.Name == "Food");
        food.ID.Should().NotBe(Guid.Empty);
        food.Name.Should().Be("Food");
        food.Cost.Should().Be(125.5m);

        var gas = mitch.Expenses.First(expense => expense.Name == "Gas");
        gas.ID.Should().NotBe(Guid.Empty);
        gas.Name.Should().Be("Gas");
        gas.Cost.Should().Be(15m);

        var nate = trip.Travelers.First(traveler => traveler.Name == "Nate");
        nate.ID.Should().NotBe(Guid.Empty);
        nate.Expenses.Should().HaveCount(1);

        var beer = nate.Expenses.First();
        beer.ID.Should().NotBe(Guid.Empty);
        beer.Name.Should().Be("Beer");
        beer.Cost.Should().Be(250m);
    }

    [Fact]
    public async Task GetTripAsync_Returns404IfTripDoesNotExist()
    {
        // Arrange
        var app = new TripExpenseCalculatorWebApplicationFactory();
        var client = app.CreateClient();

        // Act
        var response = await client.GetAsync("/api/trips/" + Guid.NewGuid().ToString());

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTripAsync_Returns400IfBadFormat()
    {
        // Arrange
        var app = new TripExpenseCalculatorWebApplicationFactory();
        var client = app.CreateClient();

        // Act
        var response = await client.GetAsync("/api/trips/" + "hello");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PutTripAsync_UpdatesTheTrip()
    {
        //This test is by far the test that takes the longest to run (70%+ of total time)
        //If this were a real app I'd definitely profile this to determine why it's taking so long.
        //Figure out if it's just the test and if it needs improved or if it's the actual updating
        //of the context that takes so long and there's a performance issue.

        // Arrange
        var app = new TripExpenseCalculatorWebApplicationFactory();
        var client = app.CreateClient();

        // Act
        var getResponse = await client.GetAsync("/api/trips/" + MagicStrings.FunTripId.ToString());

        var originalTrip = await getResponse.Content.ReadFromJsonAsync<TripDTO>();

        #region Edit Trip Details
        originalTrip.Name = "New Trip Name";
        #endregion

        #region Edit Existing Traveler
        var mitch = originalTrip.Travelers.First(traveler => traveler.Name == "Mitch");
        var nate = originalTrip.Travelers.First(traveler => traveler.Name == "Nate");
        var mitchId = mitch.ID; //Need this to make sure the id didn't change later.
        mitch.Name = "Motch";

        var food = mitch.Expenses.First(expense => expense.Name == "Food");
        var foodID = food.ID; //Need this to make sure the id didn't change later.
        food.Name = "Burgers";
        food.Cost = 200.05m;

        var gas = mitch.Expenses.First(expense => expense.Name == "Gas");
        mitch.Expenses.Remove(gas);

        var newMitchExpense = new ExpenseDTO { Name = "New Motch Expense", Cost = 999 };
        mitch.Expenses.Add(newMitchExpense);
        #endregion

        #region Add New Traveler
        var newExpenses = new List<ExpenseDTO>
        {
            new ExpenseDTO { Cost = 25m, Name = "New Traveler Expense" }
        };
        var newTraveler = new TravelerDTO { Name = "New Traveler", Expenses = newExpenses };
        originalTrip.Travelers.Add(newTraveler);
        #endregion

        #region Delete Existing Traveler
        originalTrip.Travelers.Remove(nate);
        #endregion

        var content = new StringContent(JsonConvert.SerializeObject(originalTrip), Encoding.UTF8, "application/json");

        var putResponse = await client.PutAsync("/api/trips/" + MagicStrings.FunTripId.ToString(), content);

        // Assert
        putResponse.EnsureSuccessStatusCode();

        var updatedTrip = await putResponse.Content.ReadFromJsonAsync<TripDTO>();

        updatedTrip.ID.Should().Be(MagicStrings.FunTripId);
        updatedTrip.Name.Should().Be("New Trip Name");
        updatedTrip.Travelers.Should().HaveCount(2);

        var motch = updatedTrip.Travelers.First(traveler => traveler.Name == "Motch");
        motch.ID.Should().Be(mitchId);
        motch.Expenses.Should().HaveCount(2);

        var burgers = motch.Expenses.First(expense => expense.Name == "Burgers");
        burgers.ID.Should().NotBe(Guid.Empty);
        burgers.Name.Should().Be("Burgers");
        burgers.Cost.Should().Be(200.05m);

        var newMotchExpense = motch.Expenses.First(expense => expense.Name == "New Motch Expense");
        newMotchExpense.ID.Should().NotBe(Guid.Empty);
        newMotchExpense.Name.Should().Be("New Motch Expense");
        newMotchExpense.Cost.Should().Be(999m);

        var updatedNewTraveler = updatedTrip.Travelers.First(traveler => traveler.Name == "New Traveler");
        updatedNewTraveler.ID.Should().NotBe(Guid.Empty);
        updatedNewTraveler.Expenses.Should().HaveCount(1);

        var updatedNewTravelerExpense = updatedNewTraveler.Expenses.First();
        updatedNewTravelerExpense.ID.Should().NotBe(Guid.Empty);
        updatedNewTravelerExpense.Name.Should().Be("New Traveler Expense");
        updatedNewTravelerExpense.Cost.Should().Be(25m);

    }

    [Fact]
    public async Task PostTripAsync_CreatesTheTrip()
    {
        // Arrange
        var app = new TripExpenseCalculatorWebApplicationFactory();
        var client = app.CreateClient();

        // Act
        var newTrip = new TripDTO();
        newTrip.Name = "New Trip Name";

        var newExpenses = new List<ExpenseDTO>
        {
            new ExpenseDTO { Cost = 25m, Name = "New Traveler Expense" }
        };
        var newTraveler = new TravelerDTO { Name = "New Traveler", Expenses = newExpenses };
        newTrip.Travelers.Add(newTraveler);

        var content = new StringContent(JsonConvert.SerializeObject(newTrip), Encoding.UTF8, "application/json");

        var postResponse = await client.PostAsync("/api/trips/", content);

        // Assert
        postResponse.EnsureSuccessStatusCode();

        var createdTrip = await postResponse.Content.ReadFromJsonAsync<TripDTO>();

        createdTrip.ID.Should().NotBe(Guid.Empty);
        createdTrip.Name.Should().Be("New Trip Name");
        createdTrip.Travelers.Should().HaveCount(1);

        var updatedNewTraveler = createdTrip.Travelers.First(traveler => traveler.Name == "New Traveler");
        updatedNewTraveler.ID.Should().NotBe(Guid.Empty);
        updatedNewTraveler.Expenses.Should().HaveCount(1);

        var updatedNewTravelerExpense = updatedNewTraveler.Expenses.First();
        updatedNewTravelerExpense.ID.Should().NotBe(Guid.Empty);
        updatedNewTravelerExpense.Name.Should().Be("New Traveler Expense");
        updatedNewTravelerExpense.Cost.Should().Be(25m);

        //Ensure nothing else changed

        var getAllResponse = await client.GetAsync("/api/trips");

        getAllResponse.EnsureSuccessStatusCode();
        var trips = await getAllResponse.Content.ReadFromJsonAsync<IEnumerable<TripDTO>>();

        trips.Should().HaveCount(3);

        var funTrip = trips.First(trip => trip.ID == MagicStrings.FunTripId);
        funTrip.ID.Should().Be(MagicStrings.FunTripId);
        funTrip.Name.Should().Be("A trip to the store");
        funTrip.Travelers.Should().HaveCount(2);

        var mitch = funTrip.Travelers.First(traveler => traveler.Name == "Mitch");
        mitch.ID.Should().NotBe(Guid.Empty);
        mitch.Expenses.Should().HaveCount(2);

        var food = mitch.Expenses.First(expense => expense.Name == "Food");
        food.ID.Should().NotBe(Guid.Empty);
        food.Name.Should().Be("Food");
        food.Cost.Should().Be(125.5m);

        var gas = mitch.Expenses.First(expense => expense.Name == "Gas");
        gas.ID.Should().NotBe(Guid.Empty);
        gas.Name.Should().Be("Gas");
        gas.Cost.Should().Be(15m);

        var nate = funTrip.Travelers.First(traveler => traveler.Name == "Nate");
        nate.ID.Should().NotBe(Guid.Empty);
        nate.Expenses.Should().HaveCount(1);

        var beer = nate.Expenses.First();
        beer.ID.Should().NotBe(Guid.Empty);
        beer.Name.Should().Be("Beer");
        beer.Cost.Should().Be(250m);

        var terribleTrip = trips.First(trip => trip.ID == MagicStrings.TerribleTripId);
        terribleTrip.ID.Should().Be(MagicStrings.TerribleTripId);
        terribleTrip.Name.Should().Be("An Incomplete Trip");
        terribleTrip.Travelers.Should().HaveCount(0);

        newTrip = trips.First(trip => trip.ID == createdTrip.ID);

        newTrip.ID.Should().NotBe(Guid.Empty);
        newTrip.Name.Should().Be("New Trip Name");
        newTrip.Travelers.Should().HaveCount(1);

        updatedNewTraveler = createdTrip.Travelers.First(traveler => traveler.Name == "New Traveler");
        updatedNewTraveler.ID.Should().NotBe(Guid.Empty);
        updatedNewTraveler.Expenses.Should().HaveCount(1);

        updatedNewTravelerExpense = updatedNewTraveler.Expenses.First();
        updatedNewTravelerExpense.ID.Should().NotBe(Guid.Empty);
        updatedNewTravelerExpense.Name.Should().Be("New Traveler Expense");
        updatedNewTravelerExpense.Cost.Should().Be(25m);

    }

    [Fact]
    public async Task DeleteTripAsync_DeletesTheTrip()
    {
        // Arrange
        var app = new TripExpenseCalculatorWebApplicationFactory();
        var client = app.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/trips/" + MagicStrings.FunTripId);

        // Assert
        response.EnsureSuccessStatusCode();

        var allTripsResponse = await client.GetAsync("/api/trips");
        var trips = await allTripsResponse.Content.ReadFromJsonAsync<IEnumerable<TripDTO>>();

        trips.Should().HaveCount(1);

        var terribleTrip = trips.First(trip => trip.ID == MagicStrings.TerribleTripId);
        terribleTrip.ID.Should().Be(MagicStrings.TerribleTripId);
        terribleTrip.Name.Should().Be("An Incomplete Trip");
        terribleTrip.Travelers.Should().HaveCount(0);
    }
}
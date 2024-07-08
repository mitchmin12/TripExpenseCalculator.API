using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TripExpenseCalculator.API;
using TripExpenseCalculator.API.Persistence;

namespace TripExpenseCalculator.Tests
{
    internal class TripExpenseCalculatorWebApplicationFactory : WebApplicationFactory<Program>
    {
        //This is just some required stuff to get the in memory DB working with the integration tests.
        //Realistically there would need to be a much cleaner way to test all of this like using Mocks
        //or spinning up actual DB environments as part of the integration tests.

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //We need something random so the tests don't conflict with the in memory DB.
            //This could be anything but a guid made the most sense to me.
            var id = Guid.NewGuid().ToString();

            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<TripExpenseCalculatorContext>));

                services.AddDbContext<TripExpenseCalculatorContext>(options => {
                    options.UseInMemoryDatabase("trip-expense-calculator-in-memory" + id);
                });
            }
            );
        }
    }
}

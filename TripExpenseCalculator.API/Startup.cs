using Microsoft.EntityFrameworkCore;
using TripExpenseCalculator.API.Domain.Repositories;
using TripExpenseCalculator.API.Domain.Services;
using TripExpenseCalculator.API.Persistence;

public class Startup
{
    public IConfiguration Configuration { get; }
    private string allowLocalOrigin = "allowLocalOrigin";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }


    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<TripExpenseCalculatorContext>(options => {
            options.UseInMemoryDatabase("trip-expense-calculator-in-memory");
        });

        services.AddScoped<ITripRepository, TripRepository>();
        services.AddScoped<ITripService, TripService>();

        services.AddCors(options =>
        {
            options.AddPolicy(name: allowLocalOrigin,
                              policy =>
                              {
                                  policy.WithOrigins("http://localhost:5173")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                              });
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAutoMapper(typeof(ModelToDTOProfile));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        using (var context = scope.ServiceProvider.GetService<TripExpenseCalculatorContext>())
        {
            context.Database.EnsureCreated();
        }


        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseCors(allowLocalOrigin);

        app.UseAuthorization();

        app.UseHttpsRedirection();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
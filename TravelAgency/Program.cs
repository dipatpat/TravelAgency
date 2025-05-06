using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using TravelAgency.Middlewares;
using TravelAgency.Repositories;
using TravelAgency.Services;

namespace TravelAgency;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddScoped<ITripsService, TripsService>(); //register dependency
        builder.Services.AddScoped<ITripsRepository, TripsRepository>(); 
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<IClientRepository, ClientRepository>();
        builder.Services.AddScoped<IClientTripRepository, ClientTripRepository>();
            
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Travel Agency",
                Version = "v1",
                Description = "Rest API for managing Travel Agency Trips",
                Contact = new OpenApiContact
                {
                    Name = "API Suppoert",
                    Email = "suppert@example.com",
                    Url = new Uri("https://example/suppert")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseGlobalExceptionHandling(); //registering my custom middleware

        app.UseSwagger();
        
        //Enable middleware to serve swagger-ui
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Travel Agency API v1");

            //Basic UI Customization
            c.DocExpansion(DocExpansion.List);
            c.DefaultModelsExpandDepth(0); //Hide schemas section by default
            c.DisplayRequestDuration(); //Show request duration
            c.EnableFilter(); //Enable filtering operration
        });

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.MapControllers();


        

        //string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        //string scriptPath = Path.Combine("Scripts", "init.sql");
        //PrintTrips(connectionString);
        
        //DatabaseInitializer.Run(connectionString, scriptPath); // ✅ Actually run it

        app.Run();
    }
     static void PrintTrips(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        var command = new SqlCommand("SELECT Name, DateFrom, DateTo FROM Trip", connection);
        using var reader = command.ExecuteReader();

        Console.WriteLine("📋 Available Trips:");
        while (reader.Read())
        {
            string name = reader.GetString(0);
            DateTime from = reader.GetDateTime(1);
            DateTime to = reader.GetDateTime(2);

            Console.WriteLine($"- {name} ({from:yyyy-MM-dd} → {to:yyyy-MM-dd})");
        }
    }
}
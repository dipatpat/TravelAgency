using Microsoft.Data.SqlClient;

namespace TravelAgency;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();
        app.MapControllers();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        

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
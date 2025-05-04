using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TravelAgency.Models;

namespace TravelAgency.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private string _connectionString;

    public ClientsController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }


    [HttpGet]
    public async Task<IActionResult> GetTripsAsync(CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString); //establishing connection
        await using var cmd = new SqlCommand(); //represents queries
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM Trip";
        await con.OpenAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var trips = new List<Trip>();
        while (await reader.ReadAsync())
        {
            int tripId = (int)reader["IdTrip"]; //[] fetching column name
            string tripName = (string)reader["Name"];
            string tripDescription = (string)reader["Description"];
            int maxPeople = (int)reader["MaxPeople"];

            var trip = new Trip
            {
                IdTrip = tripId,
                Name = tripName,
                Description = tripDescription,
                MaxPeople = maxPeople,
            };

            trips.Add(trip);

        }

        con.DisposeAsync(); //it's good to call this method at the end when i finish working with database when we are finished to release resources
        return Ok(trips);
    }
}
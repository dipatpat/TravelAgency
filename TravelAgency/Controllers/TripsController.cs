using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TravelAgency.DTOs;
using TravelAgency.Models;
using TravelAgency.Services;

namespace TravelAgency.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    private readonly string _connectionString;
    private readonly ITripsService _tripsService;

    public TripsController(IConfiguration configuration, ITripsService tripsService)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _tripsService = tripsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTripsAsync(CancellationToken cancellationToken)
    {
        var tripsToReturn = await _tripsService.GetTripsAsync(cancellationToken);
        //await using var con = new SqlConnection(_connectionString); //establishing connection
        //await using var cmd = new SqlCommand(); //represents queries
        //cmd.Connection = con;
        //cmd.CommandText = "SELECT * FROM Trip";
        //await con.OpenAsync(cancellationToken);

        //cancellationToken.ThrowIfCancellationRequested();
        //SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
        //var trips = new List<Trip>();
        //while (await reader.ReadAsync())
        //{
        //int tripId = (int) reader["IdTrip"]; //[] fetching column name
        //string tripName = (string) reader["Name"];
        //string tripDescription = (string) reader["Description"];
        //int maxPeople = (int) reader["MaxPeople"];

        //ar trip = new Trip
        //{
        //IdTrip = tripId,
        //Name = tripName,
        //Description = tripDescription,
        //MaxPeople = maxPeople,
        //};

        //trips.Add(trip);

        //}

        //con.DisposeAsync(); //it's good to call this method at the end when i finish working with database when we are finished to release resources
        return Ok(tripsToReturn);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTripAsync(int id, CancellationToken cancellationToken)
    {

        await using var con = new SqlConnection(_connectionString); //establishing connection
        await using var cmd = new SqlCommand(); //represents queries
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM Trip where IdTrip=@id";
        cmd.Parameters.AddWithValue("@id", id);

        await con.OpenAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync()) //remove while loop because I'm expecting just one customer
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
            return Ok(trip);
        }

        return NotFound("Trip not found");
    }

    [HttpPost]
    public async Task<IActionResult> CreateTripAsync(CreateTripRequest createNewTripRequest,
        CancellationToken cancellationToken)
    {
        var newtrip = await _tripsService.CreateTripAsync(createNewTripRequest, cancellationToken);
        await using var con = new SqlConnection(_connectionString); //establishing connection
        await using var cmd = new SqlCommand(); //represents queries
        cmd.Connection = con;
        cmd.CommandText = @"
                     INSERT INTO TRIP (Name, Description, DateFrom, DateTo, MaxPeople) 
                     VALUES (@Name, @Description,@DateFrom, @DateTo, @MaxPeople);
                     SELECT SCOPE_IDENTITY();
                     ";

        cmd.Parameters.AddWithValue("@Name", createNewTripRequest.Name);
        cmd.Parameters.AddWithValue("@Description", createNewTripRequest.Description);
        cmd.Parameters.AddWithValue("@DateFrom", createNewTripRequest.DepartureDate);
        cmd.Parameters.AddWithValue("@DateTo", createNewTripRequest.ArrivalDate);
        cmd.Parameters.AddWithValue("@MaxPeople", createNewTripRequest.MaxPeople);

        await con.OpenAsync(cancellationToken);

        var result = await cmd.ExecuteScalarAsync(cancellationToken); //the result of ExecuteScalarAsync is Decimal
        int tripId = Convert.ToInt32(result); //safe casting to avoid runtime cast failure

        return Ok(tripId);
    }
}
using Microsoft.Data.SqlClient;
using TravelAgency.Models;
using TravelAgency.Services;

namespace TravelAgency.Repositories;

public class TripsRepository : ITripsRepository
{
    private readonly string _connectionString;

    public TripsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        
    }
    //to implement 1. GET /api/trips
    public async Task<IEnumerable<Trip>> GetTripsAsync(CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString); 
        await using var cmd = new SqlCommand(); 
        cmd.Connection = con;
        cmd.CommandText = "SELECT  * FROM Trip";
        await con.OpenAsync(cancellationToken);
        
        cancellationToken.ThrowIfCancellationRequested();
        SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var trips = new List<Trip>();
        while (await reader.ReadAsync())
        {
            int tripId = (int) reader["IdTrip"]; //[] fetching column name
            string tripName = (string) reader["Name"];
            string tripDescription = (string) reader["Description"];
            int maxPeople = (int) reader["MaxPeople"];
            DateTime arrivalDate = (DateTime) reader["DateFrom"];
            DateTime departureDate = (DateTime) reader["DateTo"];

            var trip = new Trip
            {
                IdTrip = tripId,
                Name = tripName,
                Description = tripDescription,
                MaxPeople = maxPeople,
                DateFrom = arrivalDate,
                DateTo = departureDate
            };

            trips.Add(trip);

        }

        con.DisposeAsync(); 
        return trips;
    }

    //to create a TripDTO with countries
    //needed to implement 1. GET /api/trips
    public async Task<IEnumerable<string>> GetTripsCountriesAsync(CancellationToken cancellationToken, int tripId)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText =
            @"select Name from Country inner join Country_Trip 
              on country.IdCountry = country_trip.IdCountry where country_trip.IdTrip = @tripId";
        
        cmd.Parameters.AddWithValue("@TripId", tripId);
        await con.OpenAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var countries = new List<string>();
        while (await reader.ReadAsync())
        {
            string countryName = (string) reader["Name"];
            countries.Add(countryName);
        }
        con.DisposeAsync(); 
        return countries;
    }

    //needed to obtain a specific trip
    //needed for 4. PUT /api/clients/{id}/trips/{tripId},
    //5. DELETE /api/clients/{id}/trips/{tripId}
    public async Task<Trip> GetTripByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString); 
        await using var cmd = new SqlCommand(); 
        cmd.Connection = con;
        cmd.CommandText = "SELECT  * FROM Trip where IdTrip = @id";
        cmd.Parameters.AddWithValue("@id", id);
        await con.OpenAsync(cancellationToken);
        
        cancellationToken.ThrowIfCancellationRequested();
        SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync())
        {
            int tripId = (int) reader["IdTrip"]; 
            string tripName = (string) reader["Name"];
            string tripDescription = (string) reader["Description"];
            int maxPeople = (int) reader["MaxPeople"];
            DateTime arrivalDate = (DateTime) reader["DateFrom"];
            DateTime departureDate = (DateTime) reader["DateTo"];

            var trip = new Trip
            {
                IdTrip = tripId,
                Name = tripName,
                Description = tripDescription,
                MaxPeople = maxPeople,
                DateFrom = arrivalDate,
                DateTo = departureDate
            };

            return trip;
        }
        return null;
    }

    //needed to check if the trip is full to avoid overbooking and throw
    //a relevant exception
    public async Task<int> GetTotalPeopleRegisteredForATripAsync(int tripId, CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "select count(*) from Client_Trip where IdTrip = @tripId;";
        cmd.Parameters.AddWithValue("@tripId", tripId);
        await con.OpenAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

}


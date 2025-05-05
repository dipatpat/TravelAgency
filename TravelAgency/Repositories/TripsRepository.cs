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
    public async Task<IEnumerable<Trip>> GetTripsAsync(CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString); //establishing connection
        await using var cmd = new SqlCommand(); //represents queries
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
}


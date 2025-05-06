using Microsoft.Data.SqlClient;
using TravelAgency.DTOs;
using TravelAgency.Models;

namespace TravelAgency.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly string _connectionString;

    public ClientRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<Trip>> GetClientsTripsAsync(CancellationToken cancellationToken, int ClientId)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = @"select * from Trip 
                            inner join Client_Trip on Trip.IdTrip = Client_Trip.IdTrip 
                            where Client_Trip.IdClient = @ClientId";
        cmd.Parameters.AddWithValue("@ClientId", ClientId);
        await con.OpenAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var trips = new List<Trip>();
        while (await reader.ReadAsync(cancellationToken))
        {
            int tripId = (int)reader["IdTrip"]; //[] fetching column name
            string tripName = (string)reader["Name"];
            string tripDescription = (string)reader["Description"];
            int maxPeople = (int)reader["MaxPeople"];
            DateTime arrivalDate = (DateTime)reader["DateFrom"];
            DateTime departureDate = (DateTime)reader["DateTo"];
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

        return trips;
    }

    public async Task<Client?> GetClientByIdAsync(CancellationToken cancellationToken, int id)
    {
        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync(cancellationToken);

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "Select * from Client where IdClient = @id";
        cmd.Parameters.AddWithValue("@id", id);

        SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken))
        {
            return new Client
            {
                IdClient = (int)reader["IdClient"],
                FirstName = (string)reader["FirstName"],
                LastName = (string)reader["LastName"],
                Email = (string)reader["Email"],
                Telephone = (string)reader["Telephone"],
                Pesel = (string)reader["Pesel"],
            };
        }

        return null;
    }

    public async Task<int> GetRegisterdForTripAsync(CancellationToken cancellationToken, int idTrip, int idClient)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = @"select RegisteredAt from Client_Trip where IdClient = @idClient and IdTrip = @idTrip;";
        cmd.Parameters.AddWithValue("@idTrip", idTrip);
        cmd.Parameters.AddWithValue("@idClient", idClient);
        await con.OpenAsync(cancellationToken);
        SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return (int)reader["RegisteredAt"];
        }

        return 0;
    }

    public async Task<int?> GetPaymentDateForTripAsync(CancellationToken cancellationToken, int idTrip, int idClient)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = @"select PaymentDate from Client_Trip where IdClient = @idClient and IdTrip = @idTrip;";
        cmd.Parameters.AddWithValue("@idTrip", idTrip);
        cmd.Parameters.AddWithValue("@idClient", idClient);
        await con.OpenAsync(cancellationToken);
        SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return reader["PaymentDate"] != DBNull.Value ? (int?)reader["PaymentDate"] : null;
        }

        return null;
    }

    public async Task<int> CreateClientAsync(CreateClientRequest clientrequest, CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.Connection = con;
        cmd.CommandText = @"
                 INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) 
                 VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel);
                 SELECT SCOPE_IDENTITY();";
        cmd.Parameters.AddWithValue("@FirstName", clientrequest.FirstName);
        cmd.Parameters.AddWithValue("@LastName", clientrequest.LastName);
        cmd.Parameters.AddWithValue("@Email", clientrequest.Email);
        cmd.Parameters.AddWithValue("@Telephone", clientrequest.Telephone);
        cmd.Parameters.AddWithValue("@Pesel", clientrequest.Pesel);
        await con.OpenAsync(cancellationToken);
        var result = await cmd.ExecuteScalarAsync(cancellationToken); 
        int clientId = Convert.ToInt32(result);
        return clientId;
    }
}


        





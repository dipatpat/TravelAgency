using System.Data;
using Microsoft.Data.SqlClient;

namespace TravelAgency.Repositories;

public class ClientTripRepository : IClientTripRepository
{
    public readonly string _connectionString;
    
    public ClientTripRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    //to implement 4. PUT /api/clients/{id}/trips/{tripId}
    public async Task RegisterClientForTripAsync(int tripId, int clientId, CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        int registeredAt = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        cmd.CommandText = @"
                    INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt) 
                    VALUES (@ClientId, @TripId, @registeredAt)";
        
        cmd.Parameters.AddWithValue("@ClientId", clientId);
        cmd.Parameters.AddWithValue("@TripId", tripId);
        cmd.Parameters.AddWithValue("@registeredAt", registeredAt);
        
        await con.OpenAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        
        await cmd.ExecuteNonQueryAsync(cancellationToken);

    }
    // needed to validate if a client is registered for a trip, to avoid duplicate bookings
    //and throw a relevant exception 
    public async Task<bool> IsClientRegisteredForTripAsync(int idClient, int idTrip,
        CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = @"SELECT COUNT(*) FROM CLIENT_TRIP WHERE IdClient = @idClient AND IdTrip = @idTrip;";
        cmd.Parameters.AddWithValue("@idTrip", idTrip);
        cmd.Parameters.AddWithValue("@idClient", idClient);
        await con.OpenAsync(cancellationToken);
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        int count = Convert.ToInt32(result);
        return count > 0;
    }

    // to implement 5. DELETE /api/clients/{id}/trips/{tripId}
    public async Task RemoveClientFromTripAsync(int idTrip, int idClient, CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = @"DELETE FROM CLIENT_TRIP WHERE IdClient = @idClient AND IdTrip = @idTrip;";
        cmd.Parameters.Add("@idTrip", SqlDbType.Int).Value = idTrip;
        cmd.Parameters.Add("@idClient", SqlDbType.Int).Value = idClient;
        await con.OpenAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        int rowsAffected = await cmd.ExecuteNonQueryAsync(cancellationToken);
        if (rowsAffected == 0)
        {
            throw new InvalidOperationException("Delete failed: No matching record found.");
        }
        
    }
}
using TravelAgency.DTOs;
using TravelAgency.Models;

namespace TravelAgency.Repositories;

public interface IClientRepository
{
    Task<IEnumerable<Trip>> GetClientsTripsAsync(CancellationToken cancellationToken, int id);
    Task<Client> GetClientByIdAsync(CancellationToken cancellationToken, int id);
    
    Task<int> GetRegisterdForTripAsync(CancellationToken cancellationToken, int idTrip, int idClient);
    Task<int?> GetPaymentDateForTripAsync(CancellationToken cancellationToken, int idTrip, int idClient);
    
    Task<int> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken);
    
}
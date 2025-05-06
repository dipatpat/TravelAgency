using TravelAgency.DTOs;
using TravelAgency.Models;

namespace TravelAgency.Services;

public interface IClientService
{
    Task<IEnumerable<ClientsTripsDto>> GetClientTripsAsync(CancellationToken cancellationToken, int id);
    Task<Client> GetClientByIdAsync(int id, CancellationToken cancellationToken);
    Task<int> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken);
    Task RegisterClientForTripAsync(int clientId, int tripId, CancellationToken cancellationToken);
    
    Task RemoveClientFromTripAsync(int clientId, int tripId, CancellationToken cancellationToken);
    
}
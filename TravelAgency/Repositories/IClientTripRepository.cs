namespace TravelAgency.Repositories;

public interface IClientTripRepository
{
    Task RegisterClientForTripAsync(int tripId, int clientId, CancellationToken cancellationToken);
    Task<bool> IsClientRegisteredForTripAsync(int idClient, int idTrip, CancellationToken cancellationToken);
    Task RemoveClientFromTripAsync(int idTrip, int idClient, CancellationToken cancellationToken);


}
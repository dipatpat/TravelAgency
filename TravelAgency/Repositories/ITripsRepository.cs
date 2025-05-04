using TravelAgency.Models;

namespace TravelAgency.Repositories;

public interface ITripsRepository
{
    Task<IEnumerable<Trip>> GetTripsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetTripsCountriesAsync(CancellationToken cancellationToken, int TripId);

} 
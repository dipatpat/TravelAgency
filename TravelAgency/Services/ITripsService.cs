using TravelAgency.DTOs;
using TravelAgency.Models;

namespace TravelAgency.Services;

public interface ITripsService
{
    Task<IEnumerable<TripDto>> GetTripsAsync(CancellationToken cancellationToken);
    Task<int> CreateTripAsync(CreateTripRequest request, CancellationToken cancellationToken);
}
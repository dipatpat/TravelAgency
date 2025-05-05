using TravelAgency.DTOs;
using TravelAgency.Models;

namespace TravelAgency.Services;

public interface ITripsService
{
    Task<IEnumerable<TripDto>> GetTripsAsync(CancellationToken cancellationToken);
    //Task<int> CreateTripAsync(CreateClientRequest request, CancellationToken cancellationToken);
    Task<TripDto> GetTripByIdAsync(int id, CancellationToken cancellationToken);
}
using System.Collections.Concurrent;
using TravelAgency.DTOs;
using TravelAgency.Exceptions;
using TravelAgency.Models;
using TravelAgency.Repositories;

namespace TravelAgency.Services;

public class TripsService : ITripsService
{
    private readonly ITripsRepository _tripsRepository;
    public TripsService(ITripsRepository tripsRepository)
    {
        this._tripsRepository = tripsRepository;
    }
   public async Task<IEnumerable<TripDto>> GetTripsAsync(CancellationToken cancellationToken)
   
    {
       var trips = await _tripsRepository.GetTripsAsync(cancellationToken);
       if (trips == null)
       {
           throw new NotFoundException("No trips available.");
       }
       var dto = trips.Select(trip => new TripDto
       {
           IdTrip = trip.IdTrip,
           Name = trip.Name,
           Description = trip.Description,
           MaxPeople = trip.MaxPeople,
           StartDate = trip.DateFrom,
           EndDate = trip.DateTo,
           Countries = new List<string>()
       }).ToList();
       
       foreach (var tripDto in dto)
       {
           var countries = await _tripsRepository.GetTripsCountriesAsync(cancellationToken, tripDto.IdTrip);
           tripDto.Countries.AddRange(countries);
       }
       
       return dto;
    }

   

    public async Task<TripDto> GetTripByIdAsync(int tripId, CancellationToken cancellationToken)
    {
        if (tripId <= 0)
        {
            throw new BadRequestException("ClientId and TripId must be greater than zero.");
        }

        var trip = await _tripsRepository.GetTripByIdAsync(tripId, cancellationToken);
        if (trip == null)
        {
            throw new NotFoundException($"Trip with {tripId} not found");
        }
        var tripDto = new TripDto
        {
            IdTrip = trip.IdTrip,
            Name = trip.Name,
            Description = trip.Description,
            MaxPeople = trip.MaxPeople,
            StartDate = trip.DateFrom,
            EndDate = trip.DateTo,
            Countries = new List<string>()

        };
        
        var countries = await _tripsRepository.GetTripsCountriesAsync(cancellationToken, trip.IdTrip);
        tripDto.Countries.AddRange(countries);
        return tripDto;
    }
}
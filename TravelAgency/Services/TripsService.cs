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
       
       foreach (var trip in dto)
       {
           var countries = await _tripsRepository.GetTripsCountriesAsync(cancellationToken, trip.IdTrip);
           trip.Countries.AddRange(countries);
       }
       
       return dto;
    }

   

    public async Task<TripDto> GetTripByIdAsync(int id, CancellationToken cancellationToken)
    {
        var trip = await _tripsRepository.GetTripByIdAsync(id, cancellationToken);
        if (trip == null)
        {
            throw new NotFoundException($"Trip with {id} not found");
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
using System.Collections.Concurrent;
using TravelAgency.DTOs;
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
           DepartureDate = trip.DateFrom,
           ArrivalDate = trip.DateTo,
           Countries = new List<string>()
       }).ToList();
       
       foreach (var trip in dto)
       {
           var countries = await _tripsRepository.GetTripsCountriesAsync(cancellationToken, trip.IdTrip);
           trip.Countries.AddRange(countries);
       }
       
       return dto;
    }

    public async Task<int> CreateTripAsync(CreateTripRequest request, CancellationToken cancellationToken)
    {
//check if customer exists  
//ic fustomer is null throw new argument exception customer not found 
//var trip = await _tripsRepository.CreateTripAsync(request, cancellationToken);
//check if trip with such id exists 
//if trip exists, check how many people have been assigned there 
//if max people already assigned throw conflict repsonse , argument exception max limit reached
//await Trips_Clients add new client
        return 1;
    }
}
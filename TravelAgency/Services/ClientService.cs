using System.Runtime.InteropServices.JavaScript;
using TravelAgency.DTOs;
using TravelAgency.Exceptions;
using TravelAgency.Models;
using TravelAgency.Repositories;

namespace TravelAgency.Services;

public class ClientService : IClientService
{

    private static IClientRepository _clientRepository;
    private static ITripsRepository _tripsRepository;
    private static IClientTripRepository _clientTripRepository;

    public ClientService(IClientRepository clientRepository, ITripsRepository tripsRepository, IClientTripRepository clientTripRepository)
    {
        _clientRepository = clientRepository;
        _tripsRepository = tripsRepository;
        _clientTripRepository = clientTripRepository;
    }
    
    public async Task<IEnumerable<ClientsTripsDto>> GetClientTripsAsync(CancellationToken cancellationToken, int ClientId)
    {
        var client = await _clientRepository.GetClientByIdAsync(cancellationToken, ClientId);
        if (client == null)
        {
            throw new NotFoundException($"Client with id {ClientId} not found");
        }
        var trips = await _clientRepository.GetClientsTripsAsync(cancellationToken, ClientId);
        if (trips == null)
        {
            throw new NotFoundException($"Client with id {ClientId} has no trips");
        }
        var dto = trips.Select(trip => new ClientsTripsDto
        { 
            IdTrip = trip.IdTrip,
            Name = trip.Name,
            Description = trip.Description,
            MaxPeople = trip.MaxPeople,
            DepartureDate = trip.DateFrom,
            ArrivalDate = trip.DateTo,
        }).ToList();
       
        foreach (var trip in dto)
        {
            int registeredAt = await _clientRepository.GetRegisterdForTripAsync(cancellationToken, trip.IdTrip, ClientId);
            int? paymentDate = await _clientRepository.GetPaymentDateForTripAsync(cancellationToken, trip.IdTrip, ClientId);
            trip.registrationInformation = registeredAt;
            trip.paymentInformation = paymentDate;
        }
       
        return dto;
    }

    public async Task<Client> GetClientByIdAsync(int id, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetClientByIdAsync(cancellationToken, id);
        return client;
    }

    public async Task<int> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken)
    {
        var clientId = await _clientRepository.CreateClientAsync(request, cancellationToken);
        return clientId;
    }

    public async Task RegisterClientForTripAsync(int clientId, int tripId, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetClientByIdAsync(cancellationToken, clientId);
        if (client == null)
        {
            throw new NotFoundException($"Client with id {clientId} not found");
        }
        var trip = await _tripsRepository.GetTripByIdAsync(tripId, cancellationToken);
        if (trip == null)
        {
            throw new NotFoundException($"Trip with id {tripId} not found");
        }
        var maxPeoplePerTrip = trip.MaxPeople;
        var registeredPeopleForATrip = 
            await _tripsRepository.GetTotalPeopleRegisteredForATripAsync(tripId, cancellationToken);

        if (registeredPeopleForATrip >= maxPeoplePerTrip)
        {
            throw new ConflictException($"Trip with id {tripId} is already full. You can't register for it.");
        }
        
        var isAlreadyRegistered = await _clientTripRepository.IsClientRegisteredForTripAsync(clientId, tripId, cancellationToken);
        if (isAlreadyRegistered)
        {
            throw new ConflictException($"Client with id {clientId} has already registered for trip with id {tripId}");
        }
        
        await _clientTripRepository.RegisterClientForTripAsync(tripId, clientId, cancellationToken);
    }
}

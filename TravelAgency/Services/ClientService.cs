using System.Runtime.InteropServices.JavaScript;
using TravelAgency.DTOs;
using TravelAgency.Exceptions;
using TravelAgency.Models;
using TravelAgency.Repositories;

namespace TravelAgency.Services;

public class ClientService : IClientService
{

    private readonly IClientRepository _clientRepository;
    private readonly ITripsRepository _tripsRepository;
    private readonly IClientTripRepository _clientTripRepository;

    public ClientService(IClientRepository clientRepository, ITripsRepository tripsRepository, IClientTripRepository clientTripRepository)
    {
        _clientRepository = clientRepository;
        _tripsRepository = tripsRepository;
        _clientTripRepository = clientTripRepository;
    }
    
    public async Task<IEnumerable<ClientsTripsDto>> GetClientTripsAsync(CancellationToken cancellationToken, int clientId)
    {
        if (clientId <= 0)
        {
            throw new BadRequestException("ClientId must be greater than zero.");
        }
        
        var client = await _clientRepository.GetClientByIdAsync(cancellationToken, clientId);
        if (client == null)
        {
            throw new NotFoundException($"Client with id {clientId} not found");
        }
        var tripsList = (await _clientRepository.GetClientsTripsAsync(cancellationToken, clientId)).ToList();
        if (!tripsList.Any())
        {
            throw new NotFoundException($"Client with id {clientId} has no trips");
        }
        var dto = new List<ClientsTripsDto>();
        foreach (var trip in tripsList)
        { 
            int registeredAt = await _clientRepository.GetRegisterdForTripAsync(cancellationToken, trip.IdTrip, clientId);
            int? paymentDate = await _clientRepository.GetPaymentDateForTripAsync(cancellationToken, trip.IdTrip, clientId);
            dto.Add(new ClientsTripsDto
            {
                IdTrip = trip.IdTrip,
                Name = trip.Name,
                Description = trip.Description,
                MaxPeople = trip.MaxPeople,
                DepartureDate = trip.DateFrom,
                ArrivalDate = trip.DateTo,
                registrationInformation = registeredAt,
                paymentInformation = paymentDate
            });
        }
        
        return dto;
    }

    public async Task<Client> GetClientByIdAsync(int clientId, CancellationToken cancellationToken)
    {
        if (clientId <= 0)
        {
            throw new BadRequestException("ClientId must be greater than zero.");
        }

        var client = await _clientRepository.GetClientByIdAsync(cancellationToken, clientId);
        return client;
    }

    public async Task<int> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken)
    {
        var fields = new Dictionary<string, string?>
        {
            { "First name", request.FirstName },
            { "Last name", request.LastName },
            { "Email", request.Email }
        };

        foreach (var (fieldName, value) in fields)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BadRequestException($"{fieldName} is required.");
            if (value.Length > 120)
                throw new BadRequestException($"{fieldName} cannot exceed 120 characters.");
        }
        
        var clientId = await _clientRepository.CreateClientAsync(request, cancellationToken);
        return clientId;
    }

    public async Task RegisterClientForTripAsync(int clientId, int tripId, CancellationToken cancellationToken)
    {
        if (clientId <= 0 || tripId <= 0)
        {
            throw new BadRequestException("ClientId and TripId must be greater than zero.");
        }

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
        Console.WriteLine($"[DEBUG] Checking registration for clientId={clientId}, tripId={tripId}: {isAlreadyRegistered}");
        if (isAlreadyRegistered)
        {
            throw new ConflictException($"Client with id {clientId} has already registered for trip with id {tripId}");
        }
        
        await _clientTripRepository.RegisterClientForTripAsync(tripId, clientId, cancellationToken);
    }

    public async Task RemoveClientFromTripAsync(int clientId, int tripId, CancellationToken cancellationToken)
    {
        if (clientId <= 0 || tripId <= 0)
        {
            throw new BadRequestException("ClientId and TripId must be greater than zero.");
        }

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
        var isAlreadyRegistered = await _clientTripRepository.IsClientRegisteredForTripAsync(clientId, tripId, cancellationToken);
        if (!isAlreadyRegistered)
        {
            throw new ConflictException($"Client with id {clientId} is not registered for a trip with id {tripId}");
        }
        await _clientTripRepository.RemoveClientFromTripAsync(tripId, clientId, cancellationToken);
    }

}

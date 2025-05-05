using System.Runtime.InteropServices.JavaScript;
using TravelAgency.DTOs;
using TravelAgency.Exceptions;
using TravelAgency.Models;
using TravelAgency.Repositories;

namespace TravelAgency.Services;

public class ClientService : IClientService
{

    private static IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
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
}

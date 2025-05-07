using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TravelAgency.DTOs;
using TravelAgency.Exceptions;
using TravelAgency.Models;
using TravelAgency.Services;

namespace TravelAgency.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;   

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }
    
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientsTripAsync(CancellationToken cancellationToken, int id)
    {
        var trips = await _clientService.GetClientTripsAsync(cancellationToken, id);
        return Ok(trips);
    }
    
    //to be able to return the newly created client for 3. POST /api/clients 
    //to be able to register client for a trip 4. PUT /api/clients/{id}/trips/{tripId}
    [HttpGet("{id}")]
    [ActionName("GetClientByIdAsync")]
    public async Task<IActionResult> GetClientByIdAsync(int id, CancellationToken cancellationToken)
    {
        var client = await _clientService.GetClientByIdAsync(id, cancellationToken);
        return Ok(client);
    }

    //to implement 3. POST /api/clients
    [HttpPost]
    public async Task<IActionResult> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken)
    {
        var clientId = await _clientService.CreateClientAsync(request, cancellationToken);
        var client = await _clientService.GetClientByIdAsync(clientId, cancellationToken);
        return CreatedAtAction(
            "GetClientByIdAsync",
            new {id = clientId},
            client);
    }

    //to implement 4. PUT /api/clients/{id}/trips/{tripId}
    [HttpPut("{idClient}/trips/{tripId}")]
    public async Task<IActionResult> RegisterClientForTripAsync(int idClient, int tripId, CancellationToken cancellationToken)
    {
        await _clientService.RegisterClientForTripAsync(idClient, tripId, cancellationToken);
        return Ok($"Client with id {idClient} has been registered for a trip with id {tripId}");
    }
    
    //to implement 5. DELETE /api/clients/{id}/trips/{tripId}
    [HttpDelete("{idClient}/trips/{tripId}")]
    public async Task<IActionResult> UnRegisterClientForTripAsync(int idClient, int tripId, CancellationToken cancellationToken)
    {
        await _clientService.RemoveClientFromTripAsync(idClient, tripId, cancellationToken);
        return Ok($"Client with id {idClient} has been removed from a trip with id {tripId}");
    }

    // to fulfill the requirements of using 501 code in the project 
    [HttpGet("future-feature")]
    public IActionResult NotImplementedFeature()
    {
        throw new FeatureNotImplementedException("This feature is not implemented yet.");
    }
}
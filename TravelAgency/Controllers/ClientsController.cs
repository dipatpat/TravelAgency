using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TravelAgency.DTOs;
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
    [HttpGet("{id}")]
    [ActionName("GetClientByIdAsync")]
    public async Task<IActionResult> GetClientByIdAsync(int id, CancellationToken cancellationToken)
    {
        var client = await _clientService.GetClientByIdAsync(id, cancellationToken);
        return Ok(client);
    }

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
        
        
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TravelAgency.DTOs;
using TravelAgency.Models;
using TravelAgency.Services;

namespace TravelAgency.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    private readonly ITripsService _tripsService;

    public TripsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }

    //to implement GET /api/trips
    [HttpGet]
    public async Task<IActionResult> GetTripsAsync(CancellationToken cancellationToken)
    {
        var tripsToReturn = await _tripsService.GetTripsAsync(cancellationToken);
        
        return Ok(tripsToReturn);
    }

    //to be able to obtain the trip for PUT /api/clients/{id}/trips/{tripId} 
    //and DELETE /api/clients/{id}/trips/{tripId}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTripAsync(int id, CancellationToken cancellationToken)
    {
        var trip = await _tripsService.GetTripByIdAsync(id, cancellationToken);
        return Ok(trip);
    }

    
}
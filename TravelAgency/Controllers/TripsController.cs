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
    //private readonly string _connectionString;
    private readonly ITripsService _tripsService;

    public TripsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTripsAsync(CancellationToken cancellationToken)
    {
        var tripsToReturn = await _tripsService.GetTripsAsync(cancellationToken);
        
        return Ok(tripsToReturn);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTripAsync(int id, CancellationToken cancellationToken)
    {
        var trip = await _tripsService.GetTripByIdAsync(id, cancellationToken);
        return Ok(trip);
    }

    
}
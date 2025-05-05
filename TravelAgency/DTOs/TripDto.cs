using TravelAgency.Models;

namespace TravelAgency.DTOs;

public class TripDto
{
    public int IdTrip { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int MaxPeople { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public List<string> Countries { get; set; }
}
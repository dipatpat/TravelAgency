namespace TravelAgency.DTOs;

public class CreateTripRequest
{
    public int IdTrip { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int MaxPeople { get; set; }
    public DateTime? DepartureDate { get; set; }
    public DateTime? ArrivalDate { get; set; }
}
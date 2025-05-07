using System.ComponentModel.DataAnnotations;

namespace TravelAgency.DTOs;

public class ClientDto
{
    public int IdClient { get; set; }
    [MaxLength(120)] 
    public string FirstName { get; set; }
    [MaxLength(120)] 
    public string LastName { get; set; }
    [MaxLength(120)] 
    public string Email { get; set; }
    [MaxLength(120)] 
    public string Telephone { get; set; }
    [MaxLength(120)] 
    public string Pesel { get; set; }
    public List<TripDto> Trips { get; set; } = new ();
}

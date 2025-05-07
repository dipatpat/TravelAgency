using System.ComponentModel.DataAnnotations;

namespace TravelAgency.Models;

public class Country
{
    public int IdCountry { get; set; }
    [MaxLength(120)] 
    public String Name { get; set; }
}
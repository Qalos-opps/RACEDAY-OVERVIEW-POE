using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RaceDay.API.Models;

[Table("Events")]
public class Event
{
    [Key] public int EventID { get; set; }
    [Required] public int OrganiserID { get; set; }
    [Required, MaxLength(100)] public string EventName { get; set; } = "";
    [MaxLength(500)] public string? Description { get; set; }
    [Required, MaxLength(150)] public string Location { get; set; } = "";
    [Column(TypeName = "date")] public DateTime EventDate { get; set; }
    [Column(TypeName = "date")] public DateTime RegistrationDeadline { get; set; }
    [Required, MaxLength(20)] public string Status { get; set; } = "Upcoming";
    public DateTime CreatedAt { get; set; }
    [ForeignKey(nameof(OrganiserID))] public User? Organiser { get; set; }
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Weather> WeatherRecords { get; set; } = new List<Weather>();
}

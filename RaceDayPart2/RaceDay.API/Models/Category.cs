using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RaceDay.API.Models;

[Table("Categories")]
public class Category
{
    [Key] public int CategoryID { get; set; }
    [Required] public int EventID { get; set; }
    [Required, MaxLength(100)] public string CategoryName { get; set; } = "";
    [Column(TypeName = "decimal(6,2)")] public decimal DistanceKm { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal EntryFee { get; set; }
    [Required] public int MaximumParticipants { get; set; }
    [ForeignKey(nameof(EventID))] public Event? Event { get; set; }
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}

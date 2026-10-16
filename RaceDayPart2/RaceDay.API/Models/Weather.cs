using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RaceDay.API.Models;

[Table("Weather")]
public class Weather
{
    [Key] public int WeatherID { get; set; }
    [Required] public int EventID { get; set; }
    [Column(TypeName = "decimal(5,2)")] public decimal? Temperature { get; set; }
    [MaxLength(100)] public string? WeatherCondition { get; set; }
    [Column(TypeName = "decimal(5,2)")] public decimal? WindSpeed { get; set; }
    [Column(TypeName = "decimal(5,2)")] public decimal? RainProbability { get; set; }
    public DateTime RecordedAt { get; set; }
    [ForeignKey(nameof(EventID))] public Event? Event { get; set; }
}

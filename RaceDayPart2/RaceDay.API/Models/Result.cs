using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RaceDay.API.Models;

[Table("Results")]
public class Result
{
    [Key] public int ResultID { get; set; }
    [Required] public int EnrolmentID { get; set; }
    public TimeSpan? FinishTime { get; set; }
    public int? Position { get; set; }
    [Required, MaxLength(20)] public string ResultStatus { get; set; } = "Finished";
    public DateTime RecordedAt { get; set; }
    [ForeignKey(nameof(EnrolmentID))] public Enrolment? Enrolment { get; set; }
}

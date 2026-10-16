using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RaceDay.API.Models;

[Table("Enrolments")]
public class Enrolment
{
    [Key] public int EnrolmentID { get; set; }
    [Required] public int ParticipantID { get; set; }
    [Required] public int CategoryID { get; set; }
    public DateTime EnrolmentDate { get; set; }
    [Required, MaxLength(100)] public string EmergencyContact { get; set; } = "";
    [Required, MaxLength(20)] public string Status { get; set; } = "Active";
    [ForeignKey(nameof(ParticipantID))] public User? Participant { get; set; }
    [ForeignKey(nameof(CategoryID))] public Category? Category { get; set; }
    public Result? Result { get; set; }
}

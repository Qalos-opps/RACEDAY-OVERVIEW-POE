using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.DTOs;

public record EventCreateRequest(
    [Required, MaxLength(100)] string EventName,
    [MaxLength(500)] string? Description,
    [Required, MaxLength(150)] string Location,
    DateTime EventDate, DateTime RegistrationDeadline,
    [Required] string Status = "Upcoming");

public record CategoryCreateRequest(
    [Required, MaxLength(100)] string CategoryName,
    decimal DistanceKm, decimal EntryFee, int MaximumParticipants);

public record EnrolmentCreateRequest(
    int CategoryID, [Required, MaxLength(100)] string EmergencyContact);

public record ResultCreateRequest(
    int EnrolmentID, TimeSpan? FinishTime, int? Position, string ResultStatus = "Finished");

using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.DTOs;

public record RegisterRequest([Required, MaxLength(80)] string FirstName,
    [Required, MaxLength(80)] string LastName,
    [Required, EmailAddress, MaxLength(254)] string Email,
    [Required, MinLength(10)] string Password,
    [MaxLength(30)] string? PhoneNumber);
public record LoginRequest([Required, EmailAddress] string Email, [Required] string Password);
public record UserProfileDto(int UserID, string FirstName, string LastName, string Email, string Role, string? PhoneNumber);

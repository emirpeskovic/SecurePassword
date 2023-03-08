using System.ComponentModel.DataAnnotations;

namespace SecurePassword.Models.DTO;

/// <summary>
///     Login request data transfer object
/// </summary>
public class LoginRequest
{
    [Required]
    [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [MaxLength(20, ErrorMessage = "Password must be less than 20 characters")]
    public string Password { get; set; }
}
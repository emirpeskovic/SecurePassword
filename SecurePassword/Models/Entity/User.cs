using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SecurePassword.Models.Common;

namespace SecurePassword.Models.Entity;

public class User : BaseEntity
{
    /// <summary>
    ///     The email of the user.
    /// </summary>
    [Required]
    [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }

    /// <summary>
    ///     The password hash of the user.
    /// </summary>
    [Required]
    public string PasswordHash { get; set; }

    /// <summary>
    ///     The salt of the user.
    /// </summary>
    [Required]
    public string Salt { get; set; }
}
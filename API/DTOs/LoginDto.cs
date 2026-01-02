using System;
using System.ComponentModel.DataAnnotations;
using static API.Utils.Utils;

namespace API.DTOs;

public class LoginDto
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [RegularExpression(EmailPattern, ErrorMessage = "Email must be a valid email address")]
    private string _email = string.Empty;

    public string Email
    {
        get => _email;
        set => _email = value.ToLower();
    }

    // Password
    [Required]
    [StringLength(10, MinimumLength = 6, ErrorMessage = "Password must be at least {2} characters long")]
    [RegularExpression(PasswordPattern, ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
    public string Password { get; set; } = string.Empty;

}

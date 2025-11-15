<<<<<<< HEAD
using System.ComponentModel.DataAnnotations;

namespace FlightWatch.Application.DTOs.Auth;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

=======
using System.ComponentModel.DataAnnotations;

namespace FlightWatch.Application.DTOs.Auth;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2

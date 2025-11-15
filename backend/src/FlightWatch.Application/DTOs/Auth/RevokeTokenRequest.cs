<<<<<<< HEAD
using System.ComponentModel.DataAnnotations;

namespace FlightWatch.Application.DTOs.Auth;

public class RevokeTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

=======
using System.ComponentModel.DataAnnotations;

namespace FlightWatch.Application.DTOs.Auth;

public class RevokeTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2

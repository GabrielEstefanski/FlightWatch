using System.ComponentModel.DataAnnotations;

namespace FlightWatch.Application.DTOs.Auth;

public class RevokeTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}


<<<<<<< HEAD
using System.Text.Json.Serialization;

namespace FlightWatch.Infrastructure.ExternalServices.OpenSky.Models;

public class OpenSkyTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
}

=======
using System.Text.Json.Serialization;

namespace FlightWatch.Infrastructure.ExternalServices.OpenSky.Models;

public class OpenSkyTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2

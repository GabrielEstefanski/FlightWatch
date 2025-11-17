using System.Text.Json;
using FlightWatch.Infrastructure.Configuration;
using FlightWatch.Infrastructure.ExternalServices.OpenSky.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlightWatch.Infrastructure.ExternalServices.OpenSky;

public class OpenSkyAuthClient(
    HttpClient httpClient,
    IOptions<OpenSkySettings> settings,
    ILogger<OpenSkyAuthClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly OpenSkySettings _settings = settings.Value;
    private readonly ILogger<OpenSkyAuthClient> _logger = logger;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);

    private OpenSkyTokenResponse? _currentToken;

    public async Task<string> GetAccessTokenAsync()
    {
        await _tokenLock.WaitAsync();

        try
        {
            if (_currentToken != null && _currentToken.ExpiresAt > DateTime.UtcNow.AddMinutes(5))
            {
                return _currentToken.AccessToken;
            }

            _logger.LogInformation("Requesting new OpenSky access token");

            var tokenRequest = new Dictionary<string, string>
            {
                ["client_id"] = _settings.ClientId,
                ["client_secret"] = _settings.ClientSecret,
                ["grant_type"] = "client_credentials"
            };

            var content = new FormUrlEncodedContent(tokenRequest);
            var response = await _httpClient.PostAsync(_settings.AuthUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "Failed to get OpenSky token. StatusCode: {StatusCode}, Response: {Response}",
                    response.StatusCode,
                    errorContent);
                
                throw new Exception($"Failed to authenticate with OpenSky: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<OpenSkyTokenResponse>(responseContent);

            if (tokenResponse == null)
            {
                throw new Exception("Failed to deserialize OpenSky token response");
            }

            tokenResponse.ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            _currentToken = tokenResponse;

            _logger.LogInformation(
                "OpenSky access token obtained. ExpiresAt: {ExpiresAt}",
                _currentToken.ExpiresAt);

            return _currentToken.AccessToken;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    public void ClearToken()
    {
        _currentToken = null;
    }
}


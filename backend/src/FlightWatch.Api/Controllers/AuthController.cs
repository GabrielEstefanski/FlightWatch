using System.Diagnostics;
using System.Security.Claims;
using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs.Auth;
using FlightWatch.Application.Features.Auth.Commands.Login;
using FlightWatch.Application.Features.Auth.Commands.RefreshToken;
using FlightWatch.Application.Features.Auth.Commands.Register;
using FlightWatch.Application.Features.Auth.Commands.RevokeToken;
using FlightWatch.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightWatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator, ILogger<AuthController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(
            request.Email,
            request.Password,
            request.ConfirmPassword,
            request.FirstName,
            request.LastName,
            GetIpAddress());

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<AuthResponse>.FailureResponse(
                new ErrorDetails
                {
                    Code = result.Error.Code,
                    Message = result.Error.Message,
                    Type = result.Error.Type.ToString()
                },
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));
        }

        return Ok(ApiResponse<AuthResponse>.SuccessResponse(
            result.Value,
            Activity.Current?.Id ?? HttpContext.TraceIdentifier));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password,
            GetIpAddress());

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return Unauthorized(ApiResponse<AuthResponse>.FailureResponse(
                new ErrorDetails
                {
                    Code = result.Error.Code,
                    Message = result.Error.Message,
                    Type = result.Error.Type.ToString()
                },
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));
        }

        return Ok(ApiResponse<AuthResponse>.SuccessResponse(
            result.Value,
            Activity.Current?.Id ?? HttpContext.TraceIdentifier));
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(
            request.RefreshToken,
            GetIpAddress());

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<AuthResponse>.FailureResponse(
                new ErrorDetails
                {
                    Code = result.Error.Code,
                    Message = result.Error.Message,
                    Type = result.Error.Type.ToString()
                },
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));
        }

        return Ok(ApiResponse<AuthResponse>.SuccessResponse(
            result.Value,
            Activity.Current?.Id ?? HttpContext.TraceIdentifier));
    }

    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        var command = new RevokeTokenCommand(
            request.RefreshToken,
            GetIpAddress());

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                new ErrorDetails
                {
                    Code = result.Error.Code,
                    Message = result.Error.Message,
                    Type = result.Error.Type.ToString()
                },
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));
        }

        return Ok(ApiResponse<object>.SuccessResponse(
            new { message = "Token revoked successfully" },
            Activity.Current?.Id ?? HttpContext.TraceIdentifier));
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse<UserDto>.FailureResponse(
                new ErrorDetails
                {
                    Code = "INVALID_USER",
                    Message = "Invalid user identifier"
                },
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));
        }

        var query = new GetCurrentUserQuery(userId);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<UserDto>.FailureResponse(
                new ErrorDetails
                {
                    Code = result.Error.Code,
                    Message = result.Error.Message,
                    Type = result.Error.Type.ToString()
                },
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));
        }

        return Ok(ApiResponse<UserDto>.SuccessResponse(
            result.Value,
            Activity.Current?.Id ?? HttpContext.TraceIdentifier));
    }

    private string GetIpAddress()
    {
        if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            return forwardedFor.ToString().Split(',').FirstOrDefault()?.Trim() ?? "Unknown";
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}

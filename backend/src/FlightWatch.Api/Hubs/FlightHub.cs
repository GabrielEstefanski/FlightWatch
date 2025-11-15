using FlightWatch.Application.DTOs.FlightMonitoring;
using FlightWatch.Application.Features.FlightMonitoring.Commands.Subscribe;
using FlightWatch.Application.Features.FlightMonitoring.Commands.Unsubscribe;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FlightWatch.Api.Hubs;

public class FlightHub(IMediator mediator, ILogger<FlightHub> logger) : Hub
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<FlightHub> _logger = logger;

    public async Task SubscribeToFlight(SubscribeFlightRequest request)
    {
        var connectionId = Context.ConnectionId;
        var userId = Context.User?.Identity?.IsAuthenticated == true
            ? Guid.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString())
            : (Guid?)null;

        _logger.LogInformation(
            "Client subscribing to flight monitoring. ConnectionId: {ConnectionId}, Area: {AreaName}",
            connectionId,
            request.AreaName ?? "Custom");

        var command = new SubscribeFlightCommand(
            connectionId,
            request.AreaName,
            request.MinLatitude,
            request.MaxLatitude,
            request.MinLongitude,
            request.MaxLongitude,
            request.UpdateIntervalSeconds,
            userId);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            await Clients.Caller.SendAsync("SubscriptionConfirmed", result.Value);
            _logger.LogInformation(
                "Subscription confirmed. SubscriptionId: {SubscriptionId}",
                result.Value.Id);
        }
        else
        {
            await Clients.Caller.SendAsync("SubscriptionError", result.Error.Message);
            _logger.LogWarning(
                "Subscription failed. ConnectionId: {ConnectionId}, Error: {Error}",
                connectionId,
                result.Error.Message);
        }
    }

    public async Task UnsubscribeFromFlight()
    {
        var connectionId = Context.ConnectionId;

        _logger.LogInformation(
            "Client unsubscribing from flight monitoring. ConnectionId: {ConnectionId}",
            connectionId);

        var command = new UnsubscribeFlightCommand(connectionId);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            await Clients.Caller.SendAsync("UnsubscribeConfirmed");
        }
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation(
            "Client connected to FlightHub. ConnectionId: {ConnectionId}",
            Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;

        _logger.LogInformation(
            "Client disconnected from FlightHub. ConnectionId: {ConnectionId}",
            connectionId);

        var command = new UnsubscribeFlightCommand(connectionId);
        await _mediator.Send(command);

        await base.OnDisconnectedAsync(exception);
    }
}

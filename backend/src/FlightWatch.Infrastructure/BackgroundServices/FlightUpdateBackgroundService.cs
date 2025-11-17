using FlightWatch.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Infrastructure.BackgroundServices;

public class FlightUpdateBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<FlightUpdateBackgroundService> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<FlightUpdateBackgroundService> _logger = logger;
    private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(30);
    private readonly SemaphoreSlim _semaphore = new(5);
    private int _consecutiveErrors = 0;
    private const int MaxConsecutiveErrors = 3;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("FlightUpdateBackgroundService started with max concurrency of 5");

        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                await UpdateAllSubscriptionsAsync(stoppingToken);
                var elapsed = DateTime.UtcNow - startTime;

                _logger.LogInformation(
                    "Completed subscription update cycle in {ElapsedSeconds}s",
                    elapsed.TotalSeconds);

                _consecutiveErrors = 0;

                await Task.Delay(_updateInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("FlightUpdateBackgroundService is stopping");
                break;
            }
            catch (Exception ex)
            {
                _consecutiveErrors++;
                _logger.LogError(ex, 
                    "Error in FlightUpdateBackgroundService (consecutive errors: {ConsecutiveErrors})", 
                    _consecutiveErrors);

                var delaySeconds = _consecutiveErrors >= MaxConsecutiveErrors 
                    ? 60
                    : 5 * _consecutiveErrors;

                _logger.LogWarning(
                    "Waiting {DelaySeconds}s before retry due to errors",
                    delaySeconds);

                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), stoppingToken);
            }
        }

        _logger.LogInformation("FlightUpdateBackgroundService stopped");
    }

    private async Task UpdateAllSubscriptionsAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var subscriptionRepository = scope.ServiceProvider
            .GetRequiredService<IFlightSubscriptionRepository>();
        var monitoringService = scope.ServiceProvider
            .GetRequiredService<IFlightMonitoringService>();

        var subscriptions = await subscriptionRepository.GetActiveSubscriptionsAsync();

        var subscriptionsToUpdate = subscriptions
            .Where(s => (DateTime.UtcNow - s.LastUpdatedAt).TotalSeconds >= s.UpdateIntervalSeconds)
            .ToList();

        if (subscriptionsToUpdate.Count == 0)
        {
            _logger.LogInformation(
                "No subscriptions need updating at this time. Total active: {TotalActive}",
                subscriptions.Count);
            return;
        }

        _logger.LogInformation(
            "Updating {Count} subscriptions (out of {Total} active)",
            subscriptionsToUpdate.Count,
            subscriptions.Count);

        var tasks = subscriptionsToUpdate.Select(async subscription =>
        {
            await _semaphore.WaitAsync(stoppingToken);
            try
            {
                var freshSubscription = await subscriptionRepository.GetByIdAsync(subscription.Id);
                
                if (freshSubscription == null || !freshSubscription.IsActive)
                {
                    _logger.LogInformation(
                        "Subscription {SubscriptionId} is no longer active, skipping update",
                        subscription.Id);
                    return;
                }

                var timeSinceLastUpdate = (DateTime.UtcNow - freshSubscription.LastUpdatedAt).TotalSeconds;
                if (timeSinceLastUpdate < freshSubscription.UpdateIntervalSeconds)
                {
                    _logger.LogInformation(
                        "Subscription {SubscriptionId} was recently updated ({TimeSinceUpdate:F1}s ago), interval: {Interval}s, skipping",
                        subscription.Id,
                        timeSinceLastUpdate,
                        freshSubscription.UpdateIntervalSeconds);
                    return;
                }

                var result = await monitoringService.UpdateFlightsForSubscriptionAsync(subscription.Id);
                
                if (result.IsFailure)
                {
                    _logger.LogWarning(
                        "Failed to update subscription {SubscriptionId}: {Error}",
                        subscription.Id,
                        result.Error.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception updating subscription {SubscriptionId}",
                    subscription.Id);
            }
            finally
            {
                _semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

   public override void Dispose()
    {
        _semaphore?.Dispose();

        GC.SuppressFinalize(this);

        base.Dispose();
    }
}


using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Entities;
using FlightWatch.Domain.Events;

namespace FlightWatch.Api.Tests.Helpers;

public class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<Guid, User> _users = [];
    private readonly Dictionary<string, User> _usersByEmail = [];

    public Task<User?> GetByIdAsync(Guid id)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        _usersByEmail.TryGetValue(email.ToLower(), out var user);
        return Task.FromResult(user);
    }

    public Task<User> CreateAsync(User user)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = Guid.NewGuid();
        }
        if (user.CreatedAt == default)
        {
            user.CreatedAt = DateTime.UtcNow;
        }

        _users[user.Id] = user;
        _usersByEmail[user.Email.ToLower()] = user;
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        _users[user.Id] = user;
        _usersByEmail[user.Email.ToLower()] = user;
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string email)
    {
        return Task.FromResult(_usersByEmail.ContainsKey(email.ToLower()));
    }
}

public class InMemoryRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly Dictionary<string, RefreshToken> _tokens = [];
    private readonly Dictionary<Guid, List<RefreshToken>> _tokensByUserId = [];

    public Task<RefreshToken?> GetByTokenAsync(string token)
    {
        _tokens.TryGetValue(token, out var refreshToken);
        return Task.FromResult(refreshToken);
    }


    public Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
    {
        if (_tokensByUserId.TryGetValue(userId, out var tokens))
        {
            var activeTokens = tokens.Where(t => t.ExpiresAt > DateTime.UtcNow && t.RevokedAt == null).ToList();
            return Task.FromResult(activeTokens);
        }
        return Task.FromResult(new List<RefreshToken>());
    }

    public Task<RefreshToken> CreateAsync(RefreshToken token)
    {
        if (token.Id == Guid.Empty)
        {
            token.Id = Guid.NewGuid();
        }
        if (token.CreatedAt == default)
        {
            token.CreatedAt = DateTime.UtcNow;
        }

        _tokens[token.Token] = token;
        if (!_tokensByUserId.ContainsKey(token.UserId))
        {
            _tokensByUserId[token.UserId] = [];
        }
        _tokensByUserId[token.UserId].Add(token);
        return Task.FromResult(token);
    }

    public Task UpdateAsync(RefreshToken token)
    {
        _tokens[token.Token] = token;
        if (_tokensByUserId.TryGetValue(token.UserId, out var tokens))
        {
            var index = tokens.FindIndex(t => t.Token == token.Token);
            if (index >= 0)
            {
                tokens[index] = token;
            }
        }
        return Task.CompletedTask;
    }


    public Task RevokeAllByUserIdAsync(Guid userId, string ipAddress)
    {
        if (_tokensByUserId.TryGetValue(userId, out var tokens))
        {
            var now = DateTime.UtcNow;
            foreach (var token in tokens.Where(t => t.ExpiresAt > now && t.RevokedAt == null))
            {
                token.RevokedAt = now;
                token.RevokedByIp = ipAddress;
                _tokens[token.Token] = token;
            }
        }
        return Task.CompletedTask;
    }
}

public class InMemoryFlightSubscriptionRepository : IFlightSubscriptionRepository
{
    private readonly Dictionary<Guid, FlightSubscription> _subscriptions = [];
    private readonly Dictionary<string, FlightSubscription> _subscriptionsByConnectionId = [];

    public Task<FlightSubscription?> GetByIdAsync(Guid id)
    {
        _subscriptions.TryGetValue(id, out var subscription);
        return Task.FromResult(subscription);
    }

    public Task<FlightSubscription?> GetByConnectionIdAsync(string connectionId)
    {
        _subscriptionsByConnectionId.TryGetValue(connectionId, out var subscription);
        return Task.FromResult(subscription);
    }

    public Task<List<FlightSubscription>> GetActiveSubscriptionsAsync()
    {
        return Task.FromResult(_subscriptions.Values.Where(s => s.IsActive).ToList());
    }

    public Task<List<FlightSubscription>> GetByRouteAsync(string origin, string destination)
    {
        return Task.FromResult(_subscriptions.Values
            .Where(s => s.IsActive)
            .ToList());
    }

    public Task<FlightSubscription> CreateAsync(FlightSubscription subscription)
    {
        if (subscription.Id == Guid.Empty)
        {
            subscription.Id = Guid.NewGuid();
        }
        if (subscription.CreatedAt == default)
        {
            subscription.CreatedAt = DateTime.UtcNow;
        }
        subscription.LastUpdatedAt = DateTime.UtcNow;

        _subscriptions[subscription.Id] = subscription;
        _subscriptionsByConnectionId[subscription.ConnectionId] = subscription;
        return Task.FromResult(subscription);
    }

    public Task UpdateAsync(FlightSubscription subscription)
    {
        subscription.LastUpdatedAt = DateTime.UtcNow;
        _subscriptions[subscription.Id] = subscription;
        _subscriptionsByConnectionId[subscription.ConnectionId] = subscription;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        if (_subscriptions.TryGetValue(id, out var subscription))
        {
            _subscriptions.Remove(id);
            _subscriptionsByConnectionId.Remove(subscription.ConnectionId);
        }
        return Task.CompletedTask;
    }

    public Task DeleteByConnectionIdAsync(string connectionId)
    {
        if (_subscriptionsByConnectionId.TryGetValue(connectionId, out var subscription))
        {
            _subscriptions.Remove(subscription.Id);
            _subscriptionsByConnectionId.Remove(connectionId);
        }
        return Task.CompletedTask;
    }
}

public class InMemoryEventStore : IEventStore
{
    private readonly List<IDomainEvent> _events = [];

    public Task SaveEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IDomainEvent
    {
        _events.Add(@event);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_events.Where(e => e.EventId == aggregateId));
    }

    public Task<IEnumerable<IDomainEvent>> GetEventsByTypeAsync(Type eventType, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_events.Where(e => e.GetType() == eventType));
    }
}


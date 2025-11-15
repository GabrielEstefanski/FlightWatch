<<<<<<< HEAD
using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Entities;
using FlightWatch.Infrastructure.Data;
using FlightWatch.Infrastructure.Repositories;
using MongoDB.Bson;

namespace FlightWatch.Infrastructure.Repositories;

public class FlightSubscriptionRepository : RepositoryBase<FlightSubscription>, IFlightSubscriptionRepository
{
    public FlightSubscriptionRepository(MongoDbContext context) : base(context, "flightSubscriptions")
    {
    }

    public async Task<FlightSubscription?> GetByIdAsync(Guid id)
    {
        return await Collection.Find(Filter.Eq(s => s.Id, id)).FirstOrDefaultAsync();
    }

    public async Task<FlightSubscription?> GetByConnectionIdAsync(string connectionId)
    {
        return await Collection.Find(Filter.Eq(s => s.ConnectionId, connectionId)).FirstOrDefaultAsync();
    }

    public async Task<List<FlightSubscription>> GetActiveSubscriptionsAsync()
    {
        return await Collection.Find(Filter.Eq(s => s.IsActive, true)).ToListAsync();
    }

    public async Task<List<FlightSubscription>> GetByRouteAsync(string origin, string destination)
    {
        var filter = Filter.And(
            Filter.Eq(s => s.IsActive, true),
            Filter.Regex(s => s.AreaName, new BsonRegularExpression(origin, "i"))
        );

        return await Collection.Find(filter).ToListAsync();
    }

    public async Task<FlightSubscription> CreateAsync(FlightSubscription subscription)
    {
        if (subscription.Id == Guid.Empty)
        {
            subscription.Id = Guid.NewGuid();
        }
        
        if (subscription.CreatedAt == default)
        {
            subscription.CreatedAt = DateTime.UtcNow;
        }
        
        if (subscription.LastUpdatedAt == default)
        {
            subscription.LastUpdatedAt = DateTime.UtcNow;
        }

        await Collection.InsertOneAsync(subscription);
        return subscription;
    }

    public async Task UpdateAsync(FlightSubscription subscription)
    {
        subscription.LastUpdatedAt = DateTime.UtcNow;
        await Collection.ReplaceOneAsync(Filter.Eq(s => s.Id, subscription.Id), subscription);
    }

    public async Task DeleteAsync(Guid id)
    {
        await Collection.DeleteOneAsync(Filter.Eq(s => s.Id, id));
    }

    public async Task DeleteByConnectionIdAsync(string connectionId)
    {
        await Collection.DeleteOneAsync(Filter.Eq(s => s.ConnectionId, connectionId));
    }
}

=======
using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Entities;
using FlightWatch.Infrastructure.Data;
using FlightWatch.Infrastructure.Repositories;
using MongoDB.Bson;

namespace FlightWatch.Infrastructure.Repositories;

public class FlightSubscriptionRepository : RepositoryBase<FlightSubscription>, IFlightSubscriptionRepository
{
    public FlightSubscriptionRepository(MongoDbContext context) : base(context, "flightSubscriptions")
    {
    }

    public async Task<FlightSubscription?> GetByIdAsync(Guid id)
    {
        return await Collection.Find(Filter.Eq(s => s.Id, id)).FirstOrDefaultAsync();
    }

    public async Task<FlightSubscription?> GetByConnectionIdAsync(string connectionId)
    {
        return await Collection.Find(Filter.Eq(s => s.ConnectionId, connectionId)).FirstOrDefaultAsync();
    }

    public async Task<List<FlightSubscription>> GetActiveSubscriptionsAsync()
    {
        return await Collection.Find(Filter.Eq(s => s.IsActive, true)).ToListAsync();
    }

    public async Task<List<FlightSubscription>> GetByRouteAsync(string origin, string destination)
    {
        var filter = Filter.And(
            Filter.Eq(s => s.IsActive, true),
            Filter.Regex(s => s.AreaName, new BsonRegularExpression(origin, "i"))
        );

        return await Collection.Find(filter).ToListAsync();
    }

    public async Task<FlightSubscription> CreateAsync(FlightSubscription subscription)
    {
        if (subscription.Id == Guid.Empty)
        {
            subscription.Id = Guid.NewGuid();
        }
        
        if (subscription.CreatedAt == default)
        {
            subscription.CreatedAt = DateTime.UtcNow;
        }
        
        if (subscription.LastUpdatedAt == default)
        {
            subscription.LastUpdatedAt = DateTime.UtcNow;
        }

        await Collection.InsertOneAsync(subscription);
        return subscription;
    }

    public async Task UpdateAsync(FlightSubscription subscription)
    {
        subscription.LastUpdatedAt = DateTime.UtcNow;
        await Collection.ReplaceOneAsync(Filter.Eq(s => s.Id, subscription.Id), subscription);
    }

    public async Task DeleteAsync(Guid id)
    {
        await Collection.DeleteOneAsync(Filter.Eq(s => s.Id, id));
    }

    public async Task DeleteByConnectionIdAsync(string connectionId)
    {
        await Collection.DeleteOneAsync(Filter.Eq(s => s.ConnectionId, connectionId));
    }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2

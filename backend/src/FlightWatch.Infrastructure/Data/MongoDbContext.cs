using FlightWatch.Domain.Entities;
using FlightWatch.Domain.Events;
using MongoDB.Driver;

namespace FlightWatch.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient mongoClient, string databaseName)
    {
        _database = mongoClient.GetDatabase(databaseName);
        ConfigureIndexes();
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<RefreshToken> RefreshTokens => _database.GetCollection<RefreshToken>("refreshTokens");
    public IMongoCollection<FlightSubscription> FlightSubscriptions => _database.GetCollection<FlightSubscription>("flightSubscriptions");
    public IMongoCollection<StoredEvent> Events => _database.GetCollection<StoredEvent>("events");

    public IMongoCollection<T> GetCollection<T>(string collectionName) => _database.GetCollection<T>(collectionName);

    private void ConfigureIndexes()
    {
        var usersIndexes = Builders<User>.IndexKeys;
        Users.Indexes.CreateOne(new CreateIndexModel<User>(usersIndexes.Ascending(u => u.Email), new CreateIndexOptions { Unique = true }));

        var refreshTokensIndexes = Builders<RefreshToken>.IndexKeys;
        RefreshTokens.Indexes.CreateOne(new CreateIndexModel<RefreshToken>(refreshTokensIndexes.Ascending(t => t.Token)));
        RefreshTokens.Indexes.CreateOne(new CreateIndexModel<RefreshToken>(refreshTokensIndexes.Ascending(t => t.UserId)));

        var subscriptionsIndexes = Builders<FlightSubscription>.IndexKeys;
        FlightSubscriptions.Indexes.CreateOne(new CreateIndexModel<FlightSubscription>(subscriptionsIndexes.Ascending(s => s.ConnectionId)));
        FlightSubscriptions.Indexes.CreateOne(new CreateIndexModel<FlightSubscription>(subscriptionsIndexes.Ascending(s => s.UserId)));
        FlightSubscriptions.Indexes.CreateOne(new CreateIndexModel<FlightSubscription>(subscriptionsIndexes.Ascending(s => s.IsActive)));

        var eventsIndexes = Builders<StoredEvent>.IndexKeys;
        Events.Indexes.CreateOne(new CreateIndexModel<StoredEvent>(eventsIndexes.Ascending(e => e.EventId)));
        Events.Indexes.CreateOne(new CreateIndexModel<StoredEvent>(eventsIndexes.Ascending(e => e.EventType)));
        Events.Indexes.CreateOne(new CreateIndexModel<StoredEvent>(eventsIndexes.Ascending(e => e.OccurredOn)));
    }

    public class StoredEvent
    {
        public Guid EventId { get; init; }
        public string EventType { get; init; } = string.Empty;
        public DateTime OccurredOn { get; init; }
        public MongoDB.Bson.BsonDocument EventData { get; init; } = null!;
    }
}


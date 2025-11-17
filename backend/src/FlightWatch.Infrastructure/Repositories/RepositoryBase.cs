using FlightWatch.Infrastructure.Data;
using MongoDB.Driver;

namespace FlightWatch.Infrastructure.Repositories;

public abstract class RepositoryBase<T>(MongoDbContext context, string collectionName) where T : class
{
    protected readonly MongoDbContext Context = context;
    protected readonly IMongoCollection<T> Collection = context.GetCollection<T>(collectionName);

    protected IMongoCollection<T> GetCollection() => Collection;

    protected FilterDefinitionBuilder<T> Filter => Builders<T>.Filter;
    protected UpdateDefinitionBuilder<T> Update => Builders<T>.Update;
    protected IndexKeysDefinitionBuilder<T> IndexKeys => Builders<T>.IndexKeys;
    protected SortDefinitionBuilder<T> Sort => Builders<T>.Sort;
}


using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Entities;
using FlightWatch.Infrastructure.Data;
using MongoDB.Driver;

namespace FlightWatch.Infrastructure.Repositories;

public class UserRepository(MongoDbContext context) : RepositoryBase<User>(context, "users"), IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await Collection.Find(Filter.Eq(u => u.Id, id)).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Collection.Find(Filter.Eq(u => u.Email, email.ToLower())).FirstOrDefaultAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = Guid.NewGuid();
        }
        
        if (user.CreatedAt == default)
        {
            user.CreatedAt = DateTime.UtcNow;
        }

        await Collection.InsertOneAsync(user);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        await Collection.ReplaceOneAsync(Filter.Eq(u => u.Id, user.Id), user);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        var count = await Collection.CountDocumentsAsync(Filter.Eq(u => u.Email, email.ToLower()));
        return count > 0;
    }
}


<<<<<<< HEAD
using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Entities;
using FlightWatch.Infrastructure.Data;
using FlightWatch.Infrastructure.Repositories;

namespace FlightWatch.Infrastructure.Repositories;

public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(MongoDbContext context) : base(context, "refreshTokens")
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await Collection.Find(Filter.Eq(t => t.Token, token)).FirstOrDefaultAsync();
    }

    public async Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        return await Collection.Find(
            Filter.And(
                Filter.Eq(t => t.UserId, userId),
                Filter.Gt(t => t.ExpiresAt, now),
                Filter.Eq(t => t.RevokedAt, null)
            )
        ).ToListAsync();
    }

    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
    {
        if (refreshToken.Id == Guid.Empty)
        {
            refreshToken.Id = Guid.NewGuid();
        }
        
        if (refreshToken.CreatedAt == default)
        {
            refreshToken.CreatedAt = DateTime.UtcNow;
        }

        await Collection.InsertOneAsync(refreshToken);
        return refreshToken;
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        await Collection.ReplaceOneAsync(Filter.Eq(t => t.Id, refreshToken.Id), refreshToken);
    }

    public async Task RevokeAllByUserIdAsync(Guid userId, string ipAddress)
    {
        var now = DateTime.UtcNow;
        var filter = Filter.And(
            Filter.Eq(t => t.UserId, userId),
            Filter.Gt(t => t.ExpiresAt, now),
            Filter.Eq(t => t.RevokedAt, null)
        );

        var updateDefinition = Update
            .Set(t => t.RevokedAt, now)
            .Set(t => t.RevokedByIp, ipAddress);

        await Collection.UpdateManyAsync(filter, updateDefinition);
    }
}

=======
using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Entities;
using FlightWatch.Infrastructure.Data;
using FlightWatch.Infrastructure.Repositories;

namespace FlightWatch.Infrastructure.Repositories;

public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(MongoDbContext context) : base(context, "refreshTokens")
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await Collection.Find(Filter.Eq(t => t.Token, token)).FirstOrDefaultAsync();
    }

    public async Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        return await Collection.Find(
            Filter.And(
                Filter.Eq(t => t.UserId, userId),
                Filter.Gt(t => t.ExpiresAt, now),
                Filter.Eq(t => t.RevokedAt, null)
            )
        ).ToListAsync();
    }

    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
    {
        if (refreshToken.Id == Guid.Empty)
        {
            refreshToken.Id = Guid.NewGuid();
        }
        
        if (refreshToken.CreatedAt == default)
        {
            refreshToken.CreatedAt = DateTime.UtcNow;
        }

        await Collection.InsertOneAsync(refreshToken);
        return refreshToken;
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        await Collection.ReplaceOneAsync(Filter.Eq(t => t.Id, refreshToken.Id), refreshToken);
    }

    public async Task RevokeAllByUserIdAsync(Guid userId, string ipAddress)
    {
        var now = DateTime.UtcNow;
        var filter = Filter.And(
            Filter.Eq(t => t.UserId, userId),
            Filter.Gt(t => t.ExpiresAt, now),
            Filter.Eq(t => t.RevokedAt, null)
        );

        var updateDefinition = Update
            .Set(t => t.RevokedAt, now)
            .Set(t => t.RevokedByIp, ipAddress);

        await Collection.UpdateManyAsync(filter, updateDefinition);
    }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2

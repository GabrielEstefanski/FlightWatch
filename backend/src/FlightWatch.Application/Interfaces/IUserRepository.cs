<<<<<<< HEAD
using FlightWatch.Domain.Entities;

namespace FlightWatch.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> ExistsAsync(string email);
}

=======
using FlightWatch.Domain.Entities;

namespace FlightWatch.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> ExistsAsync(string email);
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2

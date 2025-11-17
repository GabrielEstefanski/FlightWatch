using FlightWatch.Domain.Entities;
using Xunit;

namespace FlightWatch.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void User_Should_Be_Created_With_Valid_Data()
    {
        // Arrange
        var email = "test@example.com";
        var passwordHash = "hashed_password";

        // Act
        var user = new User
        {
            Email = email,
            PasswordHash = passwordHash
        };

        // Assert
        Assert.Equal(email, user.Email);
        Assert.Equal(passwordHash, user.PasswordHash);
    }
}


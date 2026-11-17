using Xunit;
using Microsoft.Extensions.Configuration;
using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.Auth;
using ServiceBooking.Api.Models;
using ServiceBooking.Api.Services.Implementations;
using ServiceBooking.Tests.TestHelpers;

namespace ServiceBooking.Tests.Services;

public class AuthServiceTests
{
    private (AuthService authService, AppDbContext db) SetupAuthService(string dbName)
    {
        var db = DbContextHelper.CreateInMemoryDbContext(dbName);
        var inMemoryConfig = new Dictionary<string, string?>
        {
            {"JWT:Key", "this_is_a_very_secure_test_key_for_unit_tests_1234567890"},
            {"JWT:Issuer", "TestIssuer"},
            {"JWT:Audience", "TestAudience"},
            {"JWT:ExpirationInMinutes", "60"}
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfig)
            .Build();

        var jwtService = new JwtService(configuration);
        var authService = new AuthService(db, jwtService);

        return (authService, db);
    }

    [Fact]
    public async Task LoginAsync_ValidProvider_ReturnsTokenAndRole()
    {
        var (authService, db) = SetupAuthService(nameof(LoginAsync_ValidProvider_ReturnsTokenAndRole));
        var password = "ProviderPassword123!";
        var user = new User
        {
            Id = 50,
            Username = "provider_user",
            Email = "provider@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "Provider"
        };
        var provider = new Provider { Id = 1, UserId = 50, DisplayName = "Provider User" };
        db.Users.Add(user);
        db.Providers.Add(provider);
        await db.SaveChangesAsync();

        var result = await authService.LoginAsync(new LoginRequestDto
        {
            Email = "provider@test.com",
            Password = password
        });

        Assert.NotNull(result);
        Assert.Equal("Provider", result.Role);
        Assert.Equal("provider_user", result.Username);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    }

    [Fact]
    public async Task LoginAsync_ByUsername_ReturnsToken()
    {
        var (authService, db) = SetupAuthService(nameof(LoginAsync_ByUsername_ReturnsToken));
        var password = "MySecurePassword!";
        var user = new User
        {
            Id = 51,
            Username = "my_provider_username",
            Email = "prov_email@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "Provider"
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        // Login using Username instead of Email
        var result = await authService.LoginAsync(new LoginRequestDto
        {
            Email = "my_provider_username",
            Password = password
        });

        Assert.NotNull(result);
        Assert.Equal("Provider", result.Role);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsNull()
    {
        var (authService, db) = SetupAuthService(nameof(LoginAsync_WrongPassword_ReturnsNull));
        var user = new User
        {
            Id = 52,
            Username = "prov2",
            Email = "prov2@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),
            Role = "Provider"
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var result = await authService.LoginAsync(new LoginRequestDto
        {
            Email = "prov2@test.com",
            Password = "WrongPassword"
        });

        Assert.Null(result);
    }
}

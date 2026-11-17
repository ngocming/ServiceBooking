using Xunit;
using ServiceBooking.Api.DTOs.ProviderService;
using ServiceBooking.Api.Models;
using ServiceBooking.Api.Services.Implementations;
using ServiceBooking.Tests.TestHelpers;
using ProviderServiceModel = ServiceBooking.Api.Models.ProviderService;

namespace ServiceBooking.Tests.Services;

public class ProviderServiceOfferingServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidProvider_CreatesServiceSuccessfully()
    {
        var db = DbContextHelper.CreateInMemoryDbContext(nameof(CreateAsync_ValidProvider_CreatesServiceSuccessfully));
        var user = new User
        {
            Id = 1,
            Username = "provider1",
            Email = "provider1@test.com",
            PasswordHash = "hash",
            Role = "Provider"
        };
        var provider = new Provider { Id = 10, UserId = 1, DisplayName = "Provider One" };
        db.Users.Add(user);
        db.Providers.Add(provider);
        await db.SaveChangesAsync();

        var service = new ProviderServiceOfferingService(db);
        var dto = new CreateProviderServiceDto
        {
            Name = "House Cleaning",
            Description = "Standard cleaning",
            Price = 50.0m,
            DurationMinutes = 60
        };

        var result = await service.CreateAsync(dto, userId: 1);

        Assert.NotNull(result);
        Assert.Equal("House Cleaning", result.Name);
        Assert.Equal(10, result.ProviderId);
        Assert.True(result.IsAvailable);
    }

    [Fact]
    public async Task CreateAsync_NonProviderUser_ThrowsUnauthorizedAccessException()
    {
        var db = DbContextHelper.CreateInMemoryDbContext(nameof(CreateAsync_NonProviderUser_ThrowsUnauthorizedAccessException));
        var user = new User
        {
            Id = 2,
            Username = "customer1",
            Email = "customer1@test.com",
            PasswordHash = "hash",
            Role = "Customer"
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new ProviderServiceOfferingService(db);
        var dto = new CreateProviderServiceDto
        {
            Name = "House Cleaning",
            Description = "Standard cleaning",
            Price = 50.0m,
            DurationMinutes = 60
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.CreateAsync(dto, userId: 2));
    }

    [Fact]
    public async Task UpdateAsync_DifferentProvider_ThrowsUnauthorizedAccessException()
    {
        var db = DbContextHelper.CreateInMemoryDbContext(nameof(UpdateAsync_DifferentProvider_ThrowsUnauthorizedAccessException));
        var provider1 = new Provider { Id = 1, UserId = 1, DisplayName = "Provider 1" };
        var provider2 = new Provider { Id = 2, UserId = 2, DisplayName = "Provider 2" };
        var offering = new ProviderServiceModel
        {
            Id = 100,
            ProviderId = 1, // owned by provider 1
            Name = "Original Service",
            Price = 100,
            DurationMinutes = 60
        };
        db.Providers.AddRange(provider1, provider2);
        db.ProviderServices.Add(offering);
        await db.SaveChangesAsync();

        var service = new ProviderServiceOfferingService(db);
        var updateDto = new UpdateProviderServiceDto
        {
            Name = "Hacked Service",
            Description = "Hacked",
            Price = 10,
            DurationMinutes = 30,
            IsAvailable = true
        };

        // User 2 (Provider 2) attempts to update Provider 1's service
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.UpdateAsync(userId: 2, id: 100, updateDto));
    }

    [Fact]
    public async Task UpdateAsync_NonExistentService_ThrowsKeyNotFoundException()
    {
        var db = DbContextHelper.CreateInMemoryDbContext(nameof(UpdateAsync_NonExistentService_ThrowsKeyNotFoundException));
        var provider = new Provider { Id = 1, UserId = 1, DisplayName = "Provider 1" };
        db.Providers.Add(provider);
        await db.SaveChangesAsync();

        var service = new ProviderServiceOfferingService(db);
        var updateDto = new UpdateProviderServiceDto
        {
            Name = "Nonexistent",
            Description = "Desc",
            Price = 10,
            DurationMinutes = 30,
            IsAvailable = true
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAsync(userId: 1, id: 999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_DifferentProvider_ThrowsUnauthorizedAccessException()
    {
        var db = DbContextHelper.CreateInMemoryDbContext(nameof(DeleteAsync_DifferentProvider_ThrowsUnauthorizedAccessException));
        var provider1 = new Provider { Id = 1, UserId = 1, DisplayName = "Provider 1" };
        var provider2 = new Provider { Id = 2, UserId = 2, DisplayName = "Provider 2" };
        var offering = new ProviderServiceModel
        {
            Id = 100,
            ProviderId = 1,
            Name = "Service",
            Price = 100,
            DurationMinutes = 60
        };
        db.Providers.AddRange(provider1, provider2);
        db.ProviderServices.Add(offering);
        await db.SaveChangesAsync();

        var service = new ProviderServiceOfferingService(db);

        // User 2 attempts to delete User 1's service
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.DeleteAsync(userId: 2, id: 100));
    }

    [Fact]
    public async Task DeleteAsync_OwnerProvider_DeletesSuccessfully()
    {
        var db = DbContextHelper.CreateInMemoryDbContext(nameof(DeleteAsync_OwnerProvider_DeletesSuccessfully));
        var provider = new Provider { Id = 1, UserId = 1, DisplayName = "Provider 1" };
        var offering = new ProviderServiceModel
        {
            Id = 100,
            ProviderId = 1,
            Name = "Service",
            Price = 100,
            DurationMinutes = 60
        };
        db.Providers.Add(provider);
        db.ProviderServices.Add(offering);
        await db.SaveChangesAsync();

        var service = new ProviderServiceOfferingService(db);
        var result = await service.DeleteAsync(userId: 1, id: 100);

        Assert.True(result);
        Assert.Null(await db.ProviderServices.FindAsync(100));
    }
}

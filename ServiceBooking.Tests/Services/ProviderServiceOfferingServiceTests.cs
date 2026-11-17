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

    [Fact]
    public async Task GetFilteredAsync_SearchTerm_MatchesNameOrDescription()
    {
        var db = DbContextHelper.CreateInMemoryDbContext(nameof(GetFilteredAsync_SearchTerm_MatchesNameOrDescription));
        var user = new User { Id = 1, Username = "gara1", Email = "g1@test.com", PasswordHash = "h" };
        var provider = new Provider { Id = 1, UserId = 1, DisplayName = "Gara Auto VIP", User = user };
        var service1 = new ProviderServiceModel { Id = 1, ProviderId = 1, Name = "Rửa xe bọt tuyết", Description = "Rửa sạch bóng", Price = 80, DurationMinutes = 30, IsAvailable = true };
        var service2 = new ProviderServiceModel { Id = 2, ProviderId = 1, Name = "Thay dầu động cơ", Description = "Dầu Castrol chính hãng", Price = 350, DurationMinutes = 45, IsAvailable = true };
        db.Users.Add(user);
        db.Providers.Add(provider);
        db.ProviderServices.AddRange(service1, service2);
        await db.SaveChangesAsync();

        var service = new ProviderServiceOfferingService(db);
        var filter = new ServiceFilterDto { SearchTerm = "dầu" };

        var result = await service.GetFilteredAsync(filter);

        Assert.Single(result.Items);
        Assert.Equal("Thay dầu động cơ", result.Items[0].Name);
    }

    [Fact]
    public async Task GetFilteredAsync_PriceRange_FiltersMinMaxCorrectly()
    {
        var db = DbContextHelper.CreateInMemoryDbContext(nameof(GetFilteredAsync_PriceRange_FiltersMinMaxCorrectly));
        var user = new User { Id = 1, Username = "gara1", Email = "g1@test.com", PasswordHash = "h" };
        var provider = new Provider { Id = 1, UserId = 1, DisplayName = "Gara 1", User = user };
        var s1 = new ProviderServiceModel { Id = 1, ProviderId = 1, Name = "S1", Description = "D1", Price = 50, DurationMinutes = 30, IsAvailable = true };
        var s2 = new ProviderServiceModel { Id = 2, ProviderId = 1, Name = "S2", Description = "D2", Price = 150, DurationMinutes = 30, IsAvailable = true };
        var s3 = new ProviderServiceModel { Id = 3, ProviderId = 1, Name = "S3", Description = "D3", Price = 500, DurationMinutes = 30, IsAvailable = true };
        db.Users.Add(user);
        db.Providers.Add(provider);
        db.ProviderServices.AddRange(s1, s2, s3);
        await db.SaveChangesAsync();

        var service = new ProviderServiceOfferingService(db);
        var filter = new ServiceFilterDto { MinPrice = 100, MaxPrice = 300 };

        var result = await service.GetFilteredAsync(filter);

        Assert.Single(result.Items);
        Assert.Equal(150, result.Items[0].Price);
    }

    [Fact]
    public async Task GetFilteredAsync_Pagination_ReturnsCorrectPageSlice()
    {
        var db = DbContextHelper.CreateInMemoryDbContext(nameof(GetFilteredAsync_Pagination_ReturnsCorrectPageSlice));
        var user = new User { Id = 1, Username = "gara1", Email = "g1@test.com", PasswordHash = "h" };
        var provider = new Provider { Id = 1, UserId = 1, DisplayName = "Gara 1", User = user };
        db.Users.Add(user);
        db.Providers.Add(provider);

        for (int i = 1; i <= 15; i++)
        {
            db.ProviderServices.Add(new ProviderServiceModel
            {
                Id = i,
                ProviderId = 1,
                Name = $"Service {i}",
                Description = $"Desc {i}",
                Price = 100 * i,
                DurationMinutes = 30,
                IsAvailable = true
            });
        }
        await db.SaveChangesAsync();

        var service = new ProviderServiceOfferingService(db);
        var filter = new ServiceFilterDto { PageNumber = 2, PageSize = 5 };

        var result = await service.GetFilteredAsync(filter);

        Assert.Equal(5, result.Items.Count);
        Assert.Equal(15, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(2, result.PageNumber);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public async Task GetFilteredAsync_SortingPriceAscending_OrdersCorrectly()
    {
        var db = DbContextHelper.CreateInMemoryDbContext(nameof(GetFilteredAsync_SortingPriceAscending_OrdersCorrectly));
        var user = new User { Id = 1, Username = "gara1", Email = "g1@test.com", PasswordHash = "h" };
        var provider = new Provider { Id = 1, UserId = 1, DisplayName = "Gara 1", User = user };
        var s1 = new ProviderServiceModel { Id = 1, ProviderId = 1, Name = "Expensive", Description = "D", Price = 800, DurationMinutes = 30, IsAvailable = true };
        var s2 = new ProviderServiceModel { Id = 2, ProviderId = 1, Name = "Cheap", Description = "D", Price = 50, DurationMinutes = 30, IsAvailable = true };
        var s3 = new ProviderServiceModel { Id = 3, ProviderId = 1, Name = "Medium", Description = "D", Price = 300, DurationMinutes = 30, IsAvailable = true };
        db.Users.Add(user);
        db.Providers.Add(provider);
        db.ProviderServices.AddRange(s1, s2, s3);
        await db.SaveChangesAsync();

        var service = new ProviderServiceOfferingService(db);
        var filter = new ServiceFilterDto { SortBy = "price_asc" };

        var result = await service.GetFilteredAsync(filter);

        Assert.Equal(3, result.Items.Count);
        Assert.Equal(50, result.Items[0].Price);
        Assert.Equal(300, result.Items[1].Price);
        Assert.Equal(800, result.Items[2].Price);
    }
}

using Xunit;
using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.Category;
using ServiceBooking.Api.Services.Implementations;
using ServiceBooking.Tests.TestHelpers;

namespace ServiceBooking.Tests.Services;

public class CategoryServiceTests
{
    private AppDbContext SetupBaseData(string dbName)
    {
        return DbContextHelper.CreateInMemoryDbContext(dbName);
    }

    [Fact]
    public async Task CreateAsync_ValidCategory_GeneratesSlugAndSaves()
    {
        var db = SetupBaseData(nameof(CreateAsync_ValidCategory_GeneratesSlugAndSaves));
        var service = new CategoryService(db);

        var dto = new CreateCategoryDto
        {
            Name = "Rua xe chuyen sau",
            Description = "Dich vu rua xe cao cap",
            DisplayOrder = 1
        };

        var result = await service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Rua xe chuyen sau", result.Name);
        Assert.Equal("rua-xe-chuyen-sau", result.Slug);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetAllAsync_ActiveOnly_FiltersInactiveCategories()
    {
        var db = SetupBaseData(nameof(GetAllAsync_ActiveOnly_FiltersInactiveCategories));
        var service = new CategoryService(db);

        await service.CreateAsync(new CreateCategoryDto { Name = "Active Category 1", DisplayOrder = 1 });
        var cat2 = await service.CreateAsync(new CreateCategoryDto { Name = "Inactive Category 2", DisplayOrder = 2 });

        // Deactivate cat2
        await service.UpdateAsync(cat2.Id, new UpdateCategoryDto
        {
            Name = cat2.Name,
            Slug = cat2.Slug,
            IsActive = false
        });

        var activeCategories = await service.GetAllAsync(activeOnly: true);
        var allCategories = await service.GetAllAsync(activeOnly: false);

        Assert.Single(activeCategories);
        Assert.Equal(2, allCategories.Count);
    }

    [Fact]
    public async Task UpdateAsync_DuplicateSlug_ThrowsInvalidOperationException()
    {
        var db = SetupBaseData(nameof(UpdateAsync_DuplicateSlug_ThrowsInvalidOperationException));
        var service = new CategoryService(db);

        var cat1 = await service.CreateAsync(new CreateCategoryDto { Name = "Category One", Slug = "cat-one" });
        var cat2 = await service.CreateAsync(new CreateCategoryDto { Name = "Category Two", Slug = "cat-two" });

        // Attempt to rename cat2 to have the same slug as cat1
        var updateDto = new UpdateCategoryDto
        {
            Name = "Category Two Renamed",
            Slug = "cat-one",
            IsActive = true
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateAsync(cat2.Id, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_ExistingCategory_RemovesSuccessfully()
    {
        var db = SetupBaseData(nameof(DeleteAsync_ExistingCategory_RemovesSuccessfully));
        var service = new CategoryService(db);

        var cat = await service.CreateAsync(new CreateCategoryDto { Name = "To be deleted" });

        await service.DeleteAsync(cat.Id);

        var fetched = await service.GetByIdAsync(cat.Id);
        Assert.Null(fetched);
    }
}

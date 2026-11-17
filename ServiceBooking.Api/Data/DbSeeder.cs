using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceBooking.Api.Models;

namespace ServiceBooking.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            // Ensure the database is created/migrated
            if (dbContext.Database.IsRelational())
            {
                await dbContext.Database.MigrateAsync();
            }

            var adminUsername = configuration["AdminSeed:Username"] ?? "admin";
            var adminEmail = configuration["AdminSeed:Email"] ?? "admin@admin.com";
            var adminPassword = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(adminPassword) || adminPassword.StartsWith("<SET IN"))
            {
                Console.WriteLine("[DbSeeder] Warning: Admin seed password is not configured. Skipping admin creation.");
                return;
            }

            // Seed admin user if not exists
            var adminExists = await dbContext.Users
                .AnyAsync(u => u.Username == adminUsername || u.Email == adminEmail);

            if (!adminExists)
            {
                var adminUser = new User
                {
                    Username = adminUsername,
                    Email = adminEmail,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.Users.Add(adminUser);
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"[DbSeeder] Default admin user created successfully: {adminUsername} ({adminEmail})");
            }

            // Seed initial automotive service categories if none exist
            if (!await dbContext.ServiceCategories.AnyAsync())
            {
                var categories = new List<ServiceCategory>
                {
                    new() { Name = "Rửa xe & Chăm sóc xe", Slug = "rua-xe-cham-soc", DisplayOrder = 1, Description = "Rửa xe chuyên sâu, dọn nội thất, đánh bóng, phủ ceramic" },
                    new() { Name = "Bảo dưỡng định kỳ", Slug = "bao-duong-dinh-ky", DisplayOrder = 2, Description = "Thay dầu, lọc gió, kiểm tra phanh, bảo dưỡng toàn diện" },
                    new() { Name = "Sửa chữa nhanh", Slug = "sua-chua-nhanh", DisplayOrder = 3, Description = "Sửa điện xe, điều hòa, máy gầm, ắc quy lưu động" },
                    new() { Name = "Cứu hộ 24/7", Slug = "cuu-ho-247", DisplayOrder = 4, Description = "Cứu hộ khẩn cấp, kích bình ắc quy, vá lốp tận nơi" },
                    new() { Name = "Độ xe & Phụ kiện", Slug = "do-xe-phu-kien", DisplayOrder = 5, Description = "Camera hành trình, màn hình android, dán phim cách nhiệt, âm thanh" },
                    new() { Name = "Kiểm tra & Đăng kiểm", Slug = "kiem-tra-dang-kiem", DisplayOrder = 6, Description = "Hỗ trợ đăng kiểm xe, kiểm tra tình trạng tổng quát" }
                };

                dbContext.ServiceCategories.AddRange(categories);
                await dbContext.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Initial automotive service categories seeded successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DbSeeder] Warning: Database seeding/migration skipped: {ex.Message}");
        }
    }
}

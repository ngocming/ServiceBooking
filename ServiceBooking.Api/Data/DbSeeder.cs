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

            // Seed default provider services if none exist
            if (!await dbContext.ProviderServices.AnyAsync() && await dbContext.Providers.AnyAsync())
            {
                var providers = await dbContext.Providers.ToListAsync();
                var categories = await dbContext.ServiceCategories.ToListAsync();
                var catDict = categories.ToDictionary(c => c.Slug, c => c.Id);

                var p1 = providers[0].Id;
                var p2 = providers.Count > 1 ? providers[1].Id : p1;

                var services = new List<ProviderService>
                {
                    new()
                    {
                        ProviderId = p1,
                        CategoryId = catDict.GetValueOrDefault("rua-xe-cham-soc"),
                        Name = "Rửa xe bọt tuyết & Hút bụi nội thất cao cấp",
                        Description = "Quy trình rửa xe 7 bước bọt tuyết pH trung tính, vệ sinh mâm lốp không chạm, lau khô khăn microfiber cao cấp và hút bụi làm sạch toàn bộ khoang xe.",
                        Price = 180000,
                        DurationMinutes = 45,
                        CreatedAt = DateTime.UtcNow.AddDays(-20),
                        IsAvailable = true,
                        ImageUrl = "https://images.unsplash.com/photo-1520340356584-f9917d1eea6f?w=900&auto=format&fit=crop&q=80"
                    },
                    new()
                    {
                        ProviderId = p1,
                        CategoryId = catDict.GetValueOrDefault("rua-xe-cham-soc"),
                        Name = "Vệ sinh khoang máy chuyên sâu bằng hơi nước nóng",
                        Description = "Làm sạch dầu mỡ, cặn bẩn bám dính khoang động cơ bằng máy hơi nước nhiệt độ cao áp lực nhẹ, an toàn tuyệt đối cho giắc điện và phủ dưỡng bề mặt cao su, nhựa.",
                        Price = 650000,
                        DurationMinutes = 90,
                        CreatedAt = DateTime.UtcNow.AddDays(-18),
                        IsAvailable = true,
                        ImageUrl = "https://images.unsplash.com/photo-1486006920555-c77dce18193b?w=900&auto=format&fit=crop&q=80"
                    },
                    new()
                    {
                        ProviderId = p2,
                        CategoryId = catDict.GetValueOrDefault("bao-duong-dinh-ky"),
                        Name = "Bảo dưỡng cấp 10.000 km: Thay nhớt động cơ & Lọc dầu",
                        Description = "Thay dầu nhớt tổng hợp toàn phần chính hãng, thay lọc dầu mới, vệ sinh lọc gió động cơ và kiểm tra an toàn 24 hạng mục vận hành xe.",
                        Price = 850000,
                        DurationMinutes = 60,
                        CreatedAt = DateTime.UtcNow.AddDays(-25),
                        IsAvailable = true,
                        ImageUrl = "https://images.unsplash.com/photo-1487754180451-c456f719a1fc?w=900&auto=format&fit=crop&q=80"
                    },
                    new()
                    {
                        ProviderId = p2,
                        CategoryId = catDict.GetValueOrDefault("sua-chua-nhanh"),
                        Name = "Chẩn đoán lỗi động cơ & Hệ thống điện bằng máy OBD-II",
                        Description = "Quét mã lỗi ECU chuyên sâu bằng thiết bị chẩn đoán cao cấp, kiểm tra cảm biến động cơ, hộp số, hệ thống an toàn ABS/TCS và tư vấn phương án xử lý tối ưu.",
                        Price = 200000,
                        DurationMinutes = 30,
                        CreatedAt = DateTime.UtcNow.AddDays(-19),
                        IsAvailable = true,
                        ImageUrl = "https://images.unsplash.com/photo-1619642751034-765dfdf7c58e?w=900&auto=format&fit=crop&q=80"
                    }
                };

                dbContext.ProviderServices.AddRange(services);
                await dbContext.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Initial automotive services seeded successfully.");
            }

            // Seed default weekly schedule for providers if none exist
            if (!await dbContext.ProviderAvailabilities.AnyAsync() && await dbContext.Providers.AnyAsync())
            {
                var providers = await dbContext.Providers.ToListAsync();
                var availabilities = new List<ProviderAvailability>();

                foreach (var p in providers)
                {
                    for (int day = 1; day <= 6; day++)
                    {
                        availabilities.Add(new ProviderAvailability
                        {
                            ProviderId = p.Id,
                            DayOfWeek = (DayOfWeek)day,
                            StartTime = new TimeSpan(8, 0, 0),
                            EndTime = new TimeSpan(18, 0, 0),
                            IsAvailable = true
                        });
                    }

                    availabilities.Add(new ProviderAvailability
                    {
                        ProviderId = p.Id,
                        DayOfWeek = DayOfWeek.Sunday,
                        StartTime = new TimeSpan(8, 0, 0),
                        EndTime = new TimeSpan(12, 0, 0),
                        IsAvailable = true
                    });
                }

                dbContext.ProviderAvailabilities.AddRange(availabilities);
                await dbContext.SaveChangesAsync();
                Console.WriteLine("[DbSeeder] Initial provider availabilities seeded successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DbSeeder] Warning: Database seeding/migration skipped: {ex.Message}");
        }
    }
}

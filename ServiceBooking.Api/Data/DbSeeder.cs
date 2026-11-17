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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DbSeeder] Warning: Database seeding/migration skipped: {ex.Message}");
        }
    }
}

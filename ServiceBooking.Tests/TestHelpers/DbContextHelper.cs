using Microsoft.EntityFrameworkCore;
using ServiceBooking.Api.Data;

namespace ServiceBooking.Tests.TestHelpers;

public static class DbContextHelper
{
    public static AppDbContext CreateInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new AppDbContext(options);
    }
}

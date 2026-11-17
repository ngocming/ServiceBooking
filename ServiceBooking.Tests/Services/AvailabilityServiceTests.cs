using Xunit;
using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.Availability;
using ServiceBooking.Api.Models;
using ServiceBooking.Api.Services.Implementations;
using ServiceBooking.Tests.TestHelpers;

namespace ServiceBooking.Tests.Services;

public class AvailabilityServiceTests
{
    private (AppDbContext db, Provider provider) SetupBaseData(string dbName)
    {
        var db = DbContextHelper.CreateInMemoryDbContext(dbName);

        var provUser = new User
        {
            Id = 1,
            Username = "provider_alex",
            Email = "alex@service.com",
            PasswordHash = "hash",
            Role = "Provider"
        };
        var provider = new Provider
        {
            Id = 10,
            UserId = 1,
            DisplayName = "Alex Detailing",
            User = provUser
        };

        db.Users.Add(provUser);
        db.Providers.Add(provider);
        db.SaveChanges();

        return (db, provider);
    }

    [Fact]
    public async Task SetWeeklySchedule_ValidSlots_SavesAndRetrieves()
    {
        var (db, provider) = SetupBaseData(nameof(SetWeeklySchedule_ValidSlots_SavesAndRetrieves));
        var service = new AvailabilityService(db);

        var dto = new SetWeeklyAvailabilityDto
        {
            Slots = new List<WeeklySlotDto>
            {
                new()
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    IsAvailable = true
                },
                new()
                {
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    IsAvailable = true
                }
            }
        };

        await service.SetWeeklyScheduleAsync(userId: 1, dto);

        var schedule = await service.GetProviderScheduleAsync(provider.Id);

        Assert.Equal(2, schedule.WeeklySchedule.Count);
        Assert.Contains(schedule.WeeklySchedule, s => s.DayOfWeek == DayOfWeek.Monday && s.StartTime == new TimeSpan(8, 0, 0));
        Assert.Contains(schedule.WeeklySchedule, s => s.DayOfWeek == DayOfWeek.Tuesday && s.EndTime == new TimeSpan(18, 0, 0));
    }

    [Fact]
    public async Task SetWeeklySchedule_StartTimeAfterEndTime_ThrowsArgumentException()
    {
        var (db, provider) = SetupBaseData(nameof(SetWeeklySchedule_StartTimeAfterEndTime_ThrowsArgumentException));
        var service = new AvailabilityService(db);

        var dto = new SetWeeklyAvailabilityDto
        {
            Slots = new List<WeeklySlotDto>
            {
                new()
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(17, 0, 0),
                    EndTime = new TimeSpan(8, 0, 0),
                    IsAvailable = true
                }
            }
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.SetWeeklyScheduleAsync(userId: 1, dto));
    }

    [Fact]
    public async Task IsSlotAvailable_WithinWorkingHours_ReturnsTrue()
    {
        var (db, provider) = SetupBaseData(nameof(IsSlotAvailable_WithinWorkingHours_ReturnsTrue));
        var service = new AvailabilityService(db);

        // Schedule: Monday 09:00 - 17:00
        await service.SetWeeklyScheduleAsync(userId: 1, new SetWeeklyAvailabilityDto
        {
            Slots = new List<WeeklySlotDto>
            {
                new()
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    IsAvailable = true
                }
            }
        });

        // Next Monday at 10:00 to 11:00
        var nextMonday = DateTime.UtcNow.Date.AddDays(((int)DayOfWeek.Monday - (int)DateTime.UtcNow.DayOfWeek + 7) % 7);
        if (nextMonday <= DateTime.UtcNow) nextMonday = nextMonday.AddDays(7);

        var start = nextMonday.AddHours(10);
        var end = nextMonday.AddHours(11);

        var isAvailable = await service.IsSlotAvailableAsync(provider.Id, start, end);
        Assert.True(isAvailable);
    }

    [Fact]
    public async Task IsSlotAvailable_OutsideWorkingHours_ReturnsFalse()
    {
        var (db, provider) = SetupBaseData(nameof(IsSlotAvailable_OutsideWorkingHours_ReturnsFalse));
        var service = new AvailabilityService(db);

        // Schedule: Monday 09:00 - 17:00
        await service.SetWeeklyScheduleAsync(userId: 1, new SetWeeklyAvailabilityDto
        {
            Slots = new List<WeeklySlotDto>
            {
                new()
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    IsAvailable = true
                }
            }
        });

        var nextMonday = DateTime.UtcNow.Date.AddDays(((int)DayOfWeek.Monday - (int)DateTime.UtcNow.DayOfWeek + 7) % 7);
        if (nextMonday <= DateTime.UtcNow) nextMonday = nextMonday.AddDays(7);

        // 18:00 to 19:00 (after closing)
        var start = nextMonday.AddHours(18);
        var end = nextMonday.AddHours(19);

        var isAvailable = await service.IsSlotAvailableAsync(provider.Id, start, end);
        Assert.False(isAvailable);
    }

    [Fact]
    public async Task IsSlotAvailable_OnExceptionHoliday_ReturnsFalse()
    {
        var (db, provider) = SetupBaseData(nameof(IsSlotAvailable_OnExceptionHoliday_ReturnsFalse));
        var service = new AvailabilityService(db);

        // Schedule: Monday 09:00 - 17:00
        await service.SetWeeklyScheduleAsync(userId: 1, new SetWeeklyAvailabilityDto
        {
            Slots = new List<WeeklySlotDto>
            {
                new()
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    IsAvailable = true
                }
            }
        });

        var nextMonday = DateTime.UtcNow.Date.AddDays(((int)DayOfWeek.Monday - (int)DateTime.UtcNow.DayOfWeek + 7) % 7);
        if (nextMonday <= DateTime.UtcNow) nextMonday = nextMonday.AddDays(7);

        // Add exception: Full day off on that Monday
        var exceptionDate = DateOnly.FromDateTime(nextMonday);
        await service.AddExceptionAsync(userId: 1, new CreateAvailabilityExceptionDto
        {
            Date = exceptionDate,
            IsAvailable = false,
            Reason = "National Holiday"
        });

        var start = nextMonday.AddHours(10);
        var end = nextMonday.AddHours(11);

        var isAvailable = await service.IsSlotAvailableAsync(provider.Id, start, end);
        Assert.False(isAvailable);
    }

    [Fact]
    public async Task DeleteException_RestoresNormalAvailability()
    {
        var (db, provider) = SetupBaseData(nameof(DeleteException_RestoresNormalAvailability));
        var service = new AvailabilityService(db);

        var nextMonday = DateTime.UtcNow.Date.AddDays(((int)DayOfWeek.Monday - (int)DateTime.UtcNow.DayOfWeek + 7) % 7);
        if (nextMonday <= DateTime.UtcNow) nextMonday = nextMonday.AddDays(7);

        await service.SetWeeklyScheduleAsync(userId: 1, new SetWeeklyAvailabilityDto
        {
            Slots = new List<WeeklySlotDto>
            {
                new()
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    IsAvailable = true
                }
            }
        });

        var createdException = await service.AddExceptionAsync(userId: 1, new CreateAvailabilityExceptionDto
        {
            Date = DateOnly.FromDateTime(nextMonday),
            IsAvailable = false,
            Reason = "Personal Day"
        });

        // Delete exception
        await service.DeleteExceptionAsync(userId: 1, createdException.Id);

        var start = nextMonday.AddHours(10);
        var end = nextMonday.AddHours(11);

        var isAvailable = await service.IsSlotAvailableAsync(provider.Id, start, end);
        Assert.True(isAvailable);
    }
}

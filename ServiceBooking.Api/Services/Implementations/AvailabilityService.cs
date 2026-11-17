using Microsoft.EntityFrameworkCore;
using ServiceBooking.Api.Data;
using ServiceBooking.Api.DTOs.Availability;
using ServiceBooking.Api.Models;
using ServiceBooking.Api.Services.Interfaces;

namespace ServiceBooking.Api.Services.Implementations;

public class AvailabilityService : IAvailabilityService
{
    private readonly AppDbContext _dbContext;

    public AvailabilityService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProviderScheduleResponseDto> GetProviderScheduleAsync(int providerId)
    {
        var provider = await _dbContext.Providers
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == providerId);

        if (provider == null)
        {
            throw new KeyNotFoundException("Provider not found.");
        }

        var weekly = await _dbContext.ProviderAvailabilities
            .Where(a => a.ProviderId == providerId)
            .OrderBy(a => a.DayOfWeek)
            .ThenBy(a => a.StartTime)
            .ToListAsync();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var exceptions = await _dbContext.AvailabilityExceptions
            .Where(e => e.ProviderId == providerId && e.Date >= today.AddDays(-7))
            .OrderBy(e => e.Date)
            .ToListAsync();

        return new ProviderScheduleResponseDto
        {
            ProviderId = provider.Id,
            ProviderName = provider.DisplayName ?? provider.User?.Username ?? string.Empty,
            WeeklySchedule = weekly.Select(w => new ProviderAvailabilityDto
            {
                Id = w.Id,
                DayOfWeek = w.DayOfWeek,
                StartTime = w.StartTime,
                EndTime = w.EndTime,
                IsAvailable = w.IsAvailable
            }).ToList(),
            Exceptions = exceptions.Select(e => new AvailabilityExceptionDto
            {
                Id = e.Id,
                Date = e.Date,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                IsAvailable = e.IsAvailable,
                Reason = e.Reason
            }).ToList()
        };
    }

    public async Task<ProviderScheduleResponseDto> GetMyScheduleAsync(int userId)
    {
        var provider = await _dbContext.Providers
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (provider == null)
        {
            throw new KeyNotFoundException("Provider profile not found.");
        }

        return await GetProviderScheduleAsync(provider.Id);
    }

    public async Task SetWeeklyScheduleAsync(int userId, SetWeeklyAvailabilityDto dto)
    {
        var provider = await _dbContext.Providers
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (provider == null)
        {
            throw new KeyNotFoundException("Provider profile not found.");
        }

        foreach (var slot in dto.Slots)
        {
            if (slot.StartTime >= slot.EndTime)
            {
                throw new ArgumentException($"StartTime ({slot.StartTime}) must be earlier than EndTime ({slot.EndTime}) for {slot.DayOfWeek}.");
            }
        }

        var existing = await _dbContext.ProviderAvailabilities
            .Where(a => a.ProviderId == provider.Id)
            .ToListAsync();

        _dbContext.ProviderAvailabilities.RemoveRange(existing);

        var newAvailabilities = dto.Slots.Select(s => new ProviderAvailability
        {
            ProviderId = provider.Id,
            DayOfWeek = s.DayOfWeek,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            IsAvailable = s.IsAvailable
        });

        _dbContext.ProviderAvailabilities.AddRange(newAvailabilities);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<AvailabilityExceptionDto> AddExceptionAsync(int userId, CreateAvailabilityExceptionDto dto)
    {
        var provider = await _dbContext.Providers
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (provider == null)
        {
            throw new KeyNotFoundException("Provider profile not found.");
        }

        if (dto.StartTime.HasValue && dto.EndTime.HasValue && dto.StartTime.Value >= dto.EndTime.Value)
        {
            throw new ArgumentException("StartTime must be earlier than EndTime.");
        }

        var exception = new AvailabilityException
        {
            ProviderId = provider.Id,
            Date = dto.Date,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsAvailable = dto.IsAvailable,
            Reason = dto.Reason
        };

        _dbContext.AvailabilityExceptions.Add(exception);
        await _dbContext.SaveChangesAsync();

        return new AvailabilityExceptionDto
        {
            Id = exception.Id,
            Date = exception.Date,
            StartTime = exception.StartTime,
            EndTime = exception.EndTime,
            IsAvailable = exception.IsAvailable,
            Reason = exception.Reason
        };
    }

    public async Task DeleteExceptionAsync(int userId, int exceptionId)
    {
        var provider = await _dbContext.Providers
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (provider == null)
        {
            throw new KeyNotFoundException("Provider profile not found.");
        }

        var exception = await _dbContext.AvailabilityExceptions
            .FirstOrDefaultAsync(e => e.Id == exceptionId && e.ProviderId == provider.Id);

        if (exception == null)
        {
            throw new KeyNotFoundException("Availability exception not found or does not belong to you.");
        }

        _dbContext.AvailabilityExceptions.Remove(exception);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsSlotAvailableAsync(int providerId, DateTime startAt, DateTime endAt)
    {
        var date = DateOnly.FromDateTime(startAt);
        var startTimeOfDay = startAt.TimeOfDay;
        var endTimeOfDay = endAt.TimeOfDay;

        // 1. Check for exceptions on this date
        var exception = await _dbContext.AvailabilityExceptions
            .FirstOrDefaultAsync(e => e.ProviderId == providerId && e.Date == date);

        if (exception != null)
        {
            if (!exception.IsAvailable)
            {
                // Day off / unavailable
                if (!exception.StartTime.HasValue && !exception.EndTime.HasValue)
                {
                    // Entire day unavailable
                    return false;
                }

                // Partial time unavailable
                if (exception.StartTime.HasValue && exception.EndTime.HasValue)
                {
                    var overlapsOff = startTimeOfDay < exception.EndTime.Value && endTimeOfDay > exception.StartTime.Value;
                    if (overlapsOff)
                    {
                        return false;
                    }
                }
            }
            else
            {
                // Custom available hours
                if (exception.StartTime.HasValue && exception.EndTime.HasValue)
                {
                    return startTimeOfDay >= exception.StartTime.Value && endTimeOfDay <= exception.EndTime.Value;
                }
                return true;
            }
        }

        // 2. Check weekly schedule
        var hasConfiguredWeekly = await _dbContext.ProviderAvailabilities
            .AnyAsync(a => a.ProviderId == providerId);

        if (!hasConfiguredWeekly)
        {
            // If provider hasn't configured availability yet, allow by default
            return true;
        }

        var daySlots = await _dbContext.ProviderAvailabilities
            .Where(a => a.ProviderId == providerId && a.DayOfWeek == startAt.DayOfWeek && a.IsAvailable)
            .ToListAsync();

        if (daySlots.Count == 0)
        {
            // Provider doesn't work on this day of week
            return false;
        }

        // Must fit within at least one working slot
        return daySlots.Any(slot => startTimeOfDay >= slot.StartTime && endTimeOfDay <= slot.EndTime);
    }
}

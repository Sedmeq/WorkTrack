using BusinessLogicLayer.Services;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using Models.Models.Dto;
using Models.Models.Entities;
using EmployeeAdminPortal.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class EnhancedTimeLogService : ITimeLogService
    {
        private readonly ApplicationDbContext _context;
        private const string DateTimeFormat = "dd.MM.yyyy HH:mm:ss";
        private const string DateFormat = "dd.MM.yyyy";

        public EnhancedTimeLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TimeLogDto?> CheckInAsync(Guid employeeId, CheckInDto checkInDto)
        {
            var employee = await _context.Employees
                .Include(e => e.Role)
                .Include(e => e.WorkSchedule)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null || await IsEmployeeCheckedInAsync(employeeId))
                return null;

            var timeLog = new EmployeeTimeLog
            {
                EmployeeId = employeeId,
                CheckInTime = DateTime.Now,
                Notes = checkInDto.Notes
            };

            _context.EmployeeTimeLogs.Add(timeLog);
            await _context.SaveChangesAsync();

            return ConvertToEnhancedTimeLogDto(timeLog, employee);
        }

        public async Task<TimeLogDto?> CheckOutAsync(Guid employeeId, CheckOutDto checkOutDto)
        {
            var activeSession = await _context.EmployeeTimeLogs
                .Include(t => t.Employee)
                    .ThenInclude(e => e.Role)
                .Include(t => t.Employee)
                    .ThenInclude(e => e.WorkSchedule)
                .FirstOrDefaultAsync(t => t.EmployeeId == employeeId && t.CheckOutTime == null);

            if (activeSession == null)
                return null;

            activeSession.CheckOutTime = DateTime.Now;
            activeSession.CalculateWorkDuration();

            if (!string.IsNullOrEmpty(checkOutDto.Notes))
            {
                activeSession.Notes = string.IsNullOrEmpty(activeSession.Notes)
                    ? checkOutDto.Notes
                    : $"{activeSession.Notes} | Checkout: {checkOutDto.Notes}";
            }

            await _context.SaveChangesAsync();
            return ConvertToEnhancedTimeLogDto(activeSession, activeSession.Employee);
        }

        public async Task<TimeLogDto?> GetActiveSessionAsync(Guid employeeId)
        {
            var activeSession = await _context.EmployeeTimeLogs
                .Include(t => t.Employee)
                .ThenInclude(e => e.Role)
                .Include(t => t.Employee)
                .ThenInclude(e => e.WorkSchedule)
                .FirstOrDefaultAsync(t => t.EmployeeId == employeeId && t.CheckOutTime == null);

            return activeSession != null ? ConvertToEnhancedTimeLogDto(activeSession, activeSession.Employee) : null;
        }
        
        public async Task<List<TimeLogDto>> GetTimeLogsBySubordinatesAsync(Guid bossId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.EmployeeTimeLogs
                .Include(t => t.Employee)
                    .ThenInclude(e => e.Role)
                .Include(t => t.Employee)
                    .ThenInclude(e => e.WorkSchedule)
                .Where(t => t.Employee.BossId == bossId);

            if (fromDate.HasValue)
                query = query.Where(t => t.CheckInTime.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(t => t.CheckInTime.Date <= toDate.Value.Date);

            var timeLogs = await query
                .OrderByDescending(t => t.CheckInTime)
                .ToListAsync();

            return timeLogs.Select(t => (TimeLogDto)ConvertToEnhancedTimeLogDto(t, t.Employee)).ToList();
        }

        // --- Diğer metotlar (değişiklik yok) ---
        #region Unchanged Methods
        public async Task<List<TimeLogDto>> GetEmployeeTimeLogsAsync(Guid employeeId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.EmployeeTimeLogs.Include(t => t.Employee).ThenInclude(e => e.Role).Include(t => t.Employee).ThenInclude(e => e.WorkSchedule).Where(t => t.EmployeeId == employeeId);
            if (fromDate.HasValue) query = query.Where(t => t.CheckInTime >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(t => t.CheckInTime <= toDate.Value.AddDays(1));
            var timeLogs = await query.OrderByDescending(t => t.CheckInTime).ToListAsync();
            return timeLogs.Select(t => (TimeLogDto)ConvertToEnhancedTimeLogDto(t, t.Employee)).ToList();
        }

        public async Task<List<TimeLogDto>> GetAllTimeLogsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.EmployeeTimeLogs.Include(t => t.Employee).ThenInclude(e => e.Role).Include(t => t.Employee).ThenInclude(e => e.WorkSchedule).AsQueryable();
            if (fromDate.HasValue) query = query.Where(t => t.CheckInTime >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(t => t.CheckInTime <= toDate.Value.AddDays(1));
            var timeLogs = await query.OrderByDescending(t => t.CheckInTime).ToListAsync();
            return timeLogs.Select(t => (TimeLogDto)ConvertToEnhancedTimeLogDto(t, t.Employee)).ToList();
        }

        public async Task<List<TimeLogDto>> GetTimeLogsByRoleAsync(Guid roleId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.EmployeeTimeLogs.Include(t => t.Employee).ThenInclude(e => e.Role).Include(t => t.Employee).ThenInclude(e => e.WorkSchedule).Where(t => t.Employee.RoleId == roleId);
            if (fromDate.HasValue) query = query.Where(t => t.CheckInTime >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(t => t.CheckInTime <= toDate.Value.AddDays(1));
            var timeLogs = await query.OrderByDescending(t => t.CheckInTime).ToListAsync();
            return timeLogs.Select(t => (TimeLogDto)ConvertToEnhancedTimeLogDto(t, t.Employee)).ToList();
        }
        
        public async Task<DailyTimeLogSummaryDto> GetDailyTimeLogSummaryAsync(Guid employeeId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);
            var dailyLogs = await _context.EmployeeTimeLogs.Include(t => t.Employee).ThenInclude(e => e.Role).Include(t => t.Employee).ThenInclude(e => e.WorkSchedule).Where(t => t.EmployeeId == employeeId && t.CheckInTime >= startDate && t.CheckInTime < endDate).OrderBy(t => t.CheckInTime).ToListAsync();
            var totalWorkTime = new TimeSpan(dailyLogs.Where(t => t.WorkDuration.HasValue).Sum(t => t.WorkDuration.Value.Ticks));
            return new DailyTimeLogSummaryDto { Date = date.ToString(DateFormat), DateRaw = date.Date, TotalWorkTime = FormatTimeSpan(totalWorkTime), TotalWorkTimeRaw = totalWorkTime, CheckInCount = dailyLogs.Count, TimeLogs = dailyLogs.Select(t => (TimeLogDto)ConvertToEnhancedTimeLogDto(t, t.Employee)).ToList() };
        }

        public async Task<List<TimeLogDto>> GetTimeLogsByDateRangeAsync(Guid employeeId, DateTime fromDate, DateTime toDate) => await GetEmployeeTimeLogsAsync(employeeId, fromDate, toDate);
        public async Task<bool> IsEmployeeCheckedInAsync(Guid employeeId) => await _context.EmployeeTimeLogs.AnyAsync(t => t.EmployeeId == employeeId && t.CheckOutTime == null);
        
        public async Task<TimeSpan> GetTotalWorkTimeAsync(Guid employeeId, DateTime fromDate, DateTime toDate)
        {
            var totalTicks = await _context.EmployeeTimeLogs.Where(t => t.EmployeeId == employeeId && t.CheckInTime >= fromDate && t.CheckInTime <= toDate.AddDays(1) && t.WorkDuration.HasValue).SumAsync(t => t.WorkDuration.Value.Ticks);
            return new TimeSpan(totalTicks);
        }

        private EnhancedTimeLogDto ConvertToEnhancedTimeLogDto(EmployeeTimeLog timeLog, Employee employee)
        {
            var dto = new EnhancedTimeLogDto { Id = timeLog.Id, EmployeeId = timeLog.EmployeeId, EmployeeName = employee.Username, RoleName = employee.Role?.Name, CheckInTime = timeLog.CheckInTime.ToString(DateTimeFormat), CheckInTimeRaw = timeLog.CheckInTime, CheckOutTime = timeLog.CheckOutTime?.ToString(DateTimeFormat), CheckOutTimeRaw = timeLog.CheckOutTime, WorkDuration = timeLog.WorkDuration.HasValue ? FormatTimeSpan(timeLog.WorkDuration.Value) : null, WorkDurationRaw = timeLog.WorkDuration, WorkDurationInMinutes = timeLog.WorkDuration.HasValue ? (int)timeLog.WorkDuration.Value.TotalMinutes : null, Notes = timeLog.Notes, IsCheckedOut = timeLog.IsCheckedOut };
            if (employee.WorkSchedule != null)
            {
                var schedule = employee.WorkSchedule;
                var checkInTimeOfDay = timeLog.CheckInTime.TimeOfDay;
                dto.WorkScheduleName = schedule.Name;
                dto.ExpectedStartTime = schedule.StartTime.ToString(@"hh\:mm");
                dto.ExpectedEndTime = schedule.EndTime.ToString(@"hh\:mm");
                if (checkInTimeOfDay > schedule.StartTime)
                {
                    var lateness = checkInTimeOfDay - schedule.StartTime;
                    dto.LatenessTime = FormatTimeSpan(lateness);
                    dto.IsLate = lateness > TimeSpan.FromMinutes(schedule.MaxLatenessMinutes);
                }
                else
                {
                    dto.LatenessTime = "00:00:00";
                    dto.IsLate = false;
                }
                dto.IsWithinSchedule = schedule.IsWithinWorkHours(checkInTimeOfDay);
                if (timeLog.WorkDuration.HasValue)
                {
                    var expectedWorkTime = schedule.EndTime - schedule.StartTime;
                    var efficiency = (timeLog.WorkDuration.Value.TotalMinutes / expectedWorkTime.TotalMinutes) * 100;
                    dto.WorkEfficiency = $"{efficiency:F1}%";
                }
            }
            return dto;
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1) return $"{(int)timeSpan.TotalDays} gün, {timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        #endregion
    }
}
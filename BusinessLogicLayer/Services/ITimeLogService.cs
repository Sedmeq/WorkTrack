using Models.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public interface ITimeLogService
    {
        Task<TimeLogDto?> CheckInAsync(Guid employeeId, CheckInDto checkInDto);
        Task<TimeLogDto?> CheckOutAsync(Guid employeeId, CheckOutDto checkOutDto);

        Task<TimeLogDto?> GetActiveSessionAsync(Guid employeeId);

        Task<List<TimeLogDto>> GetEmployeeTimeLogsAsync(Guid employeeId, DateTime? fromDate = null, DateTime? toDate = null);

        Task<List<TimeLogDto>> GetAllTimeLogsAsync(DateTime? fromDate = null, DateTime? toDate = null);

        Task<List<TimeLogDto>> GetTimeLogsByRoleAsync(Guid roleId, DateTime? fromDate = null, DateTime? toDate = null);

        Task<List<TimeLogDto>> GetTimeLogsByRoleSuffixAsync(string roleSuffix, DateTime? fromDate = null, DateTime? toDate = null); // Yeni method

        //Task<List<TimeLogDto>> GetTimeLogsBySubordinatesAsync(Guid bossId, DateTime? fromDate = null, DateTime? toDate = null);

        Task<DailyTimeLogSummaryDto> GetDailyTimeLogSummaryAsync(Guid employeeId, DateTime date);

        Task<List<TimeLogDto>> GetTimeLogsByDateRangeAsync(Guid employeeId, DateTime fromDate, DateTime toDate);

        Task<bool> IsEmployeeCheckedInAsync(Guid employeeId);

        Task<TimeSpan> GetTotalWorkTimeAsync(Guid employeeId, DateTime fromDate, DateTime toDate);
    }
}
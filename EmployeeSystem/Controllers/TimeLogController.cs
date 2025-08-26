using BusinessLogicLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models.Dto;
using System.Security.Claims;
using EmployeeAdminPortal.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EmployeeAdminPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TimeLogController : ControllerBase
    {
        private readonly ITimeLogService _timeLogService;
        private readonly IEmployeeService _employeeService;

        public TimeLogController(ITimeLogService timeLogService, IEmployeeService employeeService)
        {
            _timeLogService = timeLogService;
            _employeeService = employeeService;
        }

        #region Employee Operations

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto checkInDto)
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null) return Unauthorized("Employee ID not found in token");
            var result = await _timeLogService.CheckInAsync(employeeId.Value, checkInDto);
            if (result == null) return BadRequest(new { message = "Check-in failed. You may already be checked in." });
            return Ok(new { message = "Checked in successfully", data = result });
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutDto checkOutDto)
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null) return Unauthorized("Employee ID not found in token");
            var result = await _timeLogService.CheckOutAsync(employeeId.Value, checkOutDto);
            if (result == null) return BadRequest(new { message = "Check-out failed. You may not be checked in." });
            return Ok(new { message = "Checked out successfully", data = result });
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null) return Unauthorized("Employee ID not found in token");
            var isCheckedIn = await _timeLogService.IsEmployeeCheckedInAsync(employeeId.Value);
            var activeSession = await _timeLogService.GetActiveSessionAsync(employeeId.Value);
            return Ok(new { isCheckedIn, activeSession, currentTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") });
        }

        [HttpGet("my-logs")]
        public async Task<IActionResult> GetMyTimeLogs()
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null) return Unauthorized("Employee ID not found in token");
            var allLogs = await _timeLogService.GetEmployeeTimeLogsAsync(employeeId.Value);
            var groupedByDay = allLogs.GroupBy(log => log.CheckInTimeRaw.Date).OrderByDescending(g => g.Key)
                .Select(g => new { Date = g.Key.ToString("dd.MM.yyyy"), TotalWorkTime = CalculateTotalWorkTime(g.ToList()), Logs = g.ToList() });
            return Ok(new { data = groupedByDay });
        }

        #endregion

        #region Boss Operations

        [HttpGet("employee-logs")]
        public async Task<IActionResult> GetEmployeeTimeLogs()
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null) return Unauthorized("Employee not found");

            List<TimeLogDto> result;
            if (HasAdminAccess(currentEmployee))
            {
                result = await _timeLogService.GetAllTimeLogsAsync();
            }
            else if (HasGroupBossAccess(currentEmployee))
            {
                result = await _timeLogService.GetTimeLogsBySubordinatesAsync(currentEmployee.Id);
            }
            else
            {
                return Forbid("Access denied. Boss role required.");
            }

            var groupedByEmployee = result.GroupBy(log => new { log.EmployeeId, log.EmployeeName, log.RoleName })
                .Select(g => new { g.Key.EmployeeId, g.Key.EmployeeName, g.Key.RoleName, TotalWorkTime = CalculateTotalWorkTime(g.ToList()), TotalSessions = g.Count() });
            return Ok(new { data = groupedByEmployee });
        }

        [HttpGet("employee/{employeeId}/logs")]
        public async Task<IActionResult> GetSpecificEmployeeTimeLogs(Guid employeeId)
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null) return Unauthorized("Employee not found");

            var targetEmployee = await _employeeService.GetEmployeeEntityByIdAsync(employeeId);
            if (targetEmployee == null) return NotFound("Target employee not found");

            if (!CanAccessEmployeeData(currentEmployee, targetEmployee))
                return Forbid("Access denied. You can only view employees from your group.");

            var allLogs = await _timeLogService.GetEmployeeTimeLogsAsync(employeeId);
            return Ok(new { data = allLogs });
        }

        #endregion

        #region Helper Methods
        private Guid? GetCurrentEmployeeId()
        {
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(employeeIdClaim, out var employeeId) ? employeeId : null;
        }

        private async Task<Employee?> GetCurrentEmployeeAsync()
        {
            var employeeId = GetCurrentEmployeeId();
            return employeeId.HasValue ? await _employeeService.GetEmployeeEntityByIdAsync(employeeId.Value) : null;
        }

        private bool HasAdminAccess(Employee employee) => employee.Role?.Name == "Boss";
        private bool HasGroupBossAccess(Employee employee) => employee.Role?.Name?.StartsWith("Boss-") ?? false;

        private bool CanAccessEmployeeData(Employee currentEmployee, Employee targetEmployee)
        {
            if (HasAdminAccess(currentEmployee) || currentEmployee.Id == targetEmployee.Id) return true;
            if (HasGroupBossAccess(currentEmployee)) return targetEmployee.BossId == currentEmployee.Id;
            return false;
        }

        private string CalculateTotalWorkTime(List<TimeLogDto> timeLogs)
        {
            var totalTicks = timeLogs.Where(log => log.WorkDurationRaw.HasValue).Sum(log => log.WorkDurationRaw.Value.Ticks);
            var timeSpan = new TimeSpan(totalTicks);
            return $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        #endregion
    }
}
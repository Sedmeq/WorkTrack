using BusinessLogicLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models.Dto;
using System.Security.Claims;

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
            if (employeeId == null)
                return Unauthorized("Employee ID not found in token");

            var result = await _timeLogService.CheckInAsync(employeeId.Value, checkInDto);

            if (result == null)
                return BadRequest(new { message = "Check-in failed. You may already be checked in." });

            return Ok(new { message = "Checked in successfully", data = result });
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutDto checkOutDto)
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null)
                return Unauthorized("Employee ID not found in token");

            var result = await _timeLogService.CheckOutAsync(employeeId.Value, checkOutDto);

            if (result == null)
                return BadRequest(new { message = "Check-out failed. You may not be checked in." });

            return Ok(new { message = "Checked out successfully", data = result });
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null)
                return Unauthorized("Employee ID not found in token");

            var isCheckedIn = await _timeLogService.IsEmployeeCheckedInAsync(employeeId.Value);
            var activeSession = await _timeLogService.GetActiveSessionAsync(employeeId.Value);

            return Ok(new
            {
                isCheckedIn = isCheckedIn,
                activeSession = activeSession,
                currentTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")
            });
        }

        [HttpGet("my-logs")]
        public async Task<IActionResult> GetMyTimeLogs()
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null)
                return Unauthorized("Employee ID not found in token");

            var allLogs = await _timeLogService.GetEmployeeTimeLogsAsync(employeeId.Value);

            var groupedByDay = allLogs
                .GroupBy(log => log.CheckInTimeRaw.Date)
                .OrderByDescending(group => group.Key)
                .Select(group => new
                {
                    Date = group.Key.ToString("dd.MM.yyyy"),
                    DateRaw = group.Key,
                    TotalWorkTime = CalculateTotalWorkTime(group.ToList()),
                    CheckInCount = group.Count(),
                    TimeLogs = group.OrderByDescending(log => log.CheckInTimeRaw).ToList()
                })
                .ToList();

            return Ok(new
            {
                data = groupedByDay,
                totalDays = groupedByDay.Count,
                totalSessions = allLogs.Count
            });
        }

        #endregion

        #region Boss Operations

        [HttpGet("employee-logs")]
        public async Task<IActionResult> GetEmployeeTimeLogs()
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null)
                return Unauthorized("Employee not found");

            List<TimeLogDto> result;

            if (HasAdminAccess(currentEmployee))
            {
                result = await _timeLogService.GetAllTimeLogsAsync();
            }
            else if (HasGroupBossAccess(currentEmployee))
            {
                var bossRole = currentEmployee.Role?.Name;
                if (string.IsNullOrEmpty(bossRole) || !bossRole.StartsWith("Boss-"))
                {
                    return Forbid("Invalid boss role configuration.");
                }
                var roleSuffix = bossRole.Substring("Boss-".Length);
                result = await _timeLogService.GetTimeLogsByRoleSuffixAsync(roleSuffix);
            }
            else
            {
                return Forbid("Access denied. Boss role required.");
            }

            var groupedByEmployee = result
                .GroupBy(log => new { log.EmployeeId, log.EmployeeName, log.RoleName })
                .Select(group => new
                {
                    EmployeeId = group.Key.EmployeeId,
                    EmployeeName = group.Key.EmployeeName,
                    RoleName = group.Key.RoleName,
                    TotalWorkTime = CalculateTotalWorkTime(group.ToList()),
                    TotalSessions = group.Count(),
                    WorkDays = group.GroupBy(log => log.CheckInTimeRaw.Date).Count(),
                    RecentLogs = group
                        .OrderByDescending(log => log.CheckInTimeRaw)
                        .Take(10)
                        .Select(log => new
                        {
                            Date = log.CheckInTime.Split(' ')[0],
                            CheckIn = log.CheckInTime,
                            CheckOut = log.CheckOutTime,
                            Duration = log.WorkDuration,
                            Notes = log.Notes
                        })
                        .ToList()
                })
                .OrderBy(emp => emp.EmployeeName)
                .ToList();

            return Ok(new
            {
                data = groupedByEmployee,
                totalEmployees = groupedByEmployee.Count
            });
        }

        [HttpGet("employee/{employeeId}/logs")]
        public async Task<IActionResult> GetSpecificEmployeeTimeLogs(Guid employeeId)
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null)
                return Unauthorized("Employee not found");

            var targetEmployee = await _employeeService.GetEmployeeEntityByIdAsync(employeeId);
            if (targetEmployee == null)
                return NotFound("Target employee not found");

            if (!CanAccessEmployeeData(currentEmployee, targetEmployee))
                return Forbid("Access denied. You can only view employees from your group.");

            var allLogs = await _timeLogService.GetEmployeeTimeLogsAsync(employeeId);

            var groupedByDay = allLogs
                .GroupBy(log => log.CheckInTimeRaw.Date)
                .OrderByDescending(group => group.Key)
                .Select(group => new
                {
                    Date = group.Key.ToString("dd.MM.yyyy"),
                    DateRaw = group.Key,
                    TotalWorkTime = CalculateTotalWorkTime(group.ToList()),
                    CheckInCount = group.Count(),
                    TimeLogs = group.OrderByDescending(log => log.CheckInTimeRaw).ToList()
                })
                .ToList();

            return Ok(new
            {
                employeeInfo = new
                {
                    Id = targetEmployee.Id,
                    Name = targetEmployee.Username,
                    Role = targetEmployee.Role?.Name
                },
                data = groupedByDay,
                totalDays = groupedByDay.Count,
                totalSessions = allLogs.Count
            });
        }

        [HttpGet("employee/{employeeId}/status")]
        public async Task<IActionResult> GetEmployeeStatus(Guid employeeId)
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null)
                return Unauthorized("Employee not found");

            var targetEmployee = await _employeeService.GetEmployeeEntityByIdAsync(employeeId);
            if (targetEmployee == null)
                return NotFound("Target employee not found");

            if (!CanAccessEmployeeData(currentEmployee, targetEmployee))
                return Forbid("Access denied. You can only view employees from your group.");

            var isCheckedIn = await _timeLogService.IsEmployeeCheckedInAsync(employeeId);
            var activeSession = await _timeLogService.GetActiveSessionAsync(employeeId);

            return Ok(new
            {
                employeeId = employeeId,
                employeeName = targetEmployee.Username,
                roleName = targetEmployee.Role?.Name,
                isCheckedIn = isCheckedIn,
                activeSession = activeSession,
                currentTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")
            });
        }

        #endregion

        #region Helper Methods

        private Guid? GetCurrentEmployeeId()
        {
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (employeeIdClaim != null && Guid.TryParse(employeeIdClaim, out var employeeId))
            {
                return employeeId;
            }
            return null;
        }

        private async Task<EmployeeAdminPortal.Models.Entities.Employee?> GetCurrentEmployeeAsync()
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null)
                return null;

            return await _employeeService.GetEmployeeEntityByIdAsync(employeeId.Value);
        }

        private bool HasAdminAccess(EmployeeAdminPortal.Models.Entities.Employee employee)
        {
            return employee.Role?.Name == "Boss";
        }

        private bool HasGroupBossAccess(EmployeeAdminPortal.Models.Entities.Employee employee)
        {
            var roleName = employee.Role?.Name;
            return roleName != null && roleName.StartsWith("Boss-");
        }

        private bool CanAccessEmployeeData(EmployeeAdminPortal.Models.Entities.Employee currentEmployee, EmployeeAdminPortal.Models.Entities.Employee targetEmployee)
        {
            if (HasAdminAccess(currentEmployee) || currentEmployee.Id == targetEmployee.Id)
                return true;

            if (HasGroupBossAccess(currentEmployee))
            {
                var bossRole = currentEmployee.Role?.Name;
                var employeeRole = targetEmployee.Role?.Name;

                if (string.IsNullOrEmpty(bossRole) || string.IsNullOrEmpty(employeeRole)) return false;

                var roleSuffix = bossRole.Substring("Boss-".Length);
                return employeeRole.EndsWith($"-{roleSuffix}");
            }

            return false;
        }

        private string CalculateTotalWorkTime(List<TimeLogDto> timeLogs)
        {
            var totalMinutes = timeLogs
                .Where(log => log.WorkDurationInMinutes.HasValue)
                .Sum(log => log.WorkDurationInMinutes.Value);

            if (totalMinutes == 0)
                return "00:00:00";

            var totalHours = totalMinutes / 60;
            var remainingMinutes = totalMinutes % 60;

            if (totalHours >= 24)
            {
                var days = totalHours / 24;
                var hours = totalHours % 24;
                return $"{days} gün, {hours:D2}:{remainingMinutes:D2}:00";
            }

            return $"{totalHours:D2}:{remainingMinutes:D2}:00";
        }

        #endregion
    }
}

using BusinessLogicLayer.Services;
using EmployeeAdminPortal.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Model.Dto;
using System.Security.Claims;

namespace EmployeeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PermissionController : Controller
    {
        private readonly IPermissionService _permissionService;
        private readonly IEmployeeService _employeeService;
        public PermissionController(IPermissionService permissionService , IEmployeeService employeeService)
        {
            _permissionService = permissionService;
            _employeeService = employeeService;
        }
        #region Employee Endpoints

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitLeaveRequest([FromBody] CreatePermissionDto requestDto)
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null) return Unauthorized();

            var result = await _permissionService.SubmitPermissionAsync(employeeId.Value, requestDto);
            if (result == null) return BadRequest(new { message = "İcazə müraciəti göndərilə bilmədi." });

            return Ok(new { message = "İcazə müraciəti uğurla göndərildi.", data = result });
        }

        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyLeaveRequests()
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null) return Unauthorized();

            var requests = await _permissionService.GetMyPermissionsAsync(employeeId.Value);
            return Ok(new { data = requests, count = requests.Count });
        }

        #endregion

        #region Boss Endpoints

        [HttpGet("pending-for-approval")]
        public async Task<IActionResult> GetPendingLeaveRequests()
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null) return Unauthorized();

            if (!HasAdminAccess(currentEmployee) && !HasGroupBossAccess(currentEmployee))
            {
                return Forbid("Access denied. Boss role required.");
            }

            var requests = await _permissionService.GetPendingPermissionsForBossAsync(currentEmployee.Id);
            return Ok(new { data = requests, count = requests.Count });
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveLeaveRequest(Guid id)
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null) return Unauthorized();

            if (!HasAdminAccess(currentEmployee) && !HasGroupBossAccess(currentEmployee))
            {
                return Forbid("Access denied. Boss role required.");
            }

            var result = await _permissionService.ApprovePermissionAsync(id, currentEmployee.Id);
            if (!result) return BadRequest(new { message = "İcazə müraciəti təsdiq edilə bilmədi. Yoxlanılan müraciət mövcud deyil və ya siz bu işçinin rəhbəri deyilsiniz." });

            return Ok(new { message = "İcazə müraciəti uğurla təsdiqləndi." });
        }

        [HttpPost("deny/{id}")]
        public async Task<IActionResult> DenyLeaveRequest(Guid id)
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null) return Unauthorized();

            if (!HasAdminAccess(currentEmployee) && !HasGroupBossAccess(currentEmployee))
            {
                return Forbid("Access denied. Boss role required.");
            }

            var result = await _permissionService.DenyPermissionAsync(id, currentEmployee.Id);
            if (!result) return BadRequest(new { message = "İcazə müraciəti rədd edilə bilmədi. Yoxlanılan müraciət mövcud deyil və ya siz bu işçinin rəhbəri deyilsiniz." });

            return Ok(new { message = "İcazə müraciəti uğurla rədd edildi." });
        }

        [HttpPost("grant")]
        public async Task<IActionResult> GrantLeave([FromBody] BossPermissionDto grantDto)
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null) return Unauthorized();

            if (!HasAdminAccess(currentEmployee) && !HasGroupBossAccess(currentEmployee))
            {
                return Forbid("Access denied. Boss role required.");
            }

            var result = await _permissionService.GrantLeaveAsync(currentEmployee.Id, grantDto);
            if (result == null) return BadRequest(new { message = "İcazə verilə bilmədi. İşçi mövcud deyil və ya sizin rəhbərliyiniz altında deyil." });

            return Ok(new { message = "İcazə uğurla verildi.", data = result });
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
        #endregion
    }
}

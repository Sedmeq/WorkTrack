using BusinessLogicLayer.Services;
using EmployeeAdminPortal.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Model.Dto;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VacationController : ControllerBase
    {
        private readonly IVacationService _vacationService;
        private readonly IEmployeeService _employeeService;

        public VacationController(IVacationService vacationService, IEmployeeService employeeService)
        {
            _vacationService = vacationService;
            _employeeService = employeeService;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitVacationRequest([FromBody] CreateVacationDto requestDto)
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null) return Unauthorized();

            var result = await _vacationService.SubmitVacationAsync(employeeId.Value, requestDto);
            if (result == null)
            {
                return BadRequest(new { message = "Müraciət göndərilə bilmədi. Qalıq məzuniyyət günlərinizi keçirsiniz və ya rəhbəriniz təyin edilməyib." });
            }

            return Ok(new { message = "Məzuniyyət müraciətiniz uğurla göndərildi.", data = result });
        }

        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyVacationRequests()
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null) return Unauthorized();

            var requests = await _vacationService.GetMyVacationsAsync(employeeId.Value);
            return Ok(new { data = requests, count = requests.Count });
        }

        [HttpGet("pending-for-approval")]
        public async Task<IActionResult> GetPendingRequestsForBoss()
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null || !currentEmployee.IsBoss())
                return Forbid("Access denied. Boss role required.");

            var requests = await _vacationService.GetPendingVacationsForBossAsync(currentEmployee.Id);
            return Ok(new { data = requests, count = requests.Count });
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveVacation(Guid id)
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null || !currentEmployee.IsBoss()) return Forbid("Access denied.");

            var result = await _vacationService.ApproveVacationAsync(id, currentEmployee.Id);
            if (!result) return BadRequest(new { message = "Müraciət təsdiq edilə bilmədi." });

            return Ok(new { message = "Müraciət uğurla təsdiqləndi." });
        }

        [HttpPost("deny/{id}")]
        public async Task<IActionResult> DenyVacation(Guid id)
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null || !currentEmployee.IsBoss()) return Forbid("Access denied.");

            var result = await _vacationService.DenyVacationAsync(id, currentEmployee.Id);
            if (!result) return BadRequest(new { message = "Müraciət rədd edilə bilmədi." });

            return Ok(new { message = "Müraciət uğurla rədd edildi." });
        }

        //[HttpPost("grant")]
        //public async Task<IActionResult> GrantVacation([FromBody] BossVacationDto grantDto)
        //{
        //    var currentEmployee = await GetCurrentEmployeeAsync();
        //    if (currentEmployee == null || !currentEmployee.IsBoss()) return Forbid("Access denied.");

        //    var result = await _vacationService.GrantVacationAsync(currentEmployee.Id, grantDto);
        //    if (result == null) return BadRequest(new { message = "Məzuniyyət verilə bilmədi." });

        //    return Ok(new { message = "Məzuniyyət uğurla verildi.", data = result });
        //}

        // Helper metodlar
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
    }
}
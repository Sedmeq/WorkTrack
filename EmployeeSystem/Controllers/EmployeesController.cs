using BusinessLogicLayer.Services;
using EmployeeAdminPortal.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models.Dto;
using System.Security.Claims;
using EmployeeAdminPortal.Models.Entities;

namespace EmployeeAdminPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // Mövcud role-ları əldə etmək üçün yeni endpoint
        [HttpGet("available-roles")]
        public async Task<IActionResult> GetAvailableRoles()
        {
            var roles = await _employeeService.GetAvailableRolesAsync();
            return Ok(new { data = roles });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null)
                return Unauthorized("Employee not found");

            List<EmployeeResponseDto> employees;

            if (HasAdminAccess(currentEmployee))
            {
                employees = await _employeeService.GetAllEmployeesAsync();
            }
            else if (HasGroupBossAccess(currentEmployee))
            {
                var bossRole = currentEmployee.Role?.Name;
                if (string.IsNullOrEmpty(bossRole) || !bossRole.StartsWith("Boss-"))
                {
                    return Forbid("Invalid boss role configuration.");
                }

                var roleSuffix = bossRole.Substring("Boss-".Length);
                employees = await _employeeService.GetEmployeesByRoleSuffixAsync(roleSuffix);
            }
            else
            {
                var selfEmployee = await _employeeService.GetEmployeeByIdAsync(currentEmployee.Id);
                employees = selfEmployee != null ? new List<EmployeeResponseDto> { selfEmployee } : new List<EmployeeResponseDto>();
            }

            return Ok(new { data = employees, count = employees.Count });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null)
                return Unauthorized("Employee not found");

            var targetEmployee = await _employeeService.GetEmployeeEntityByIdAsync(id);
            if (targetEmployee == null)
                return NotFound(new { message = "Employee not found" });

            if (!CanAccessEmployeeData(currentEmployee, targetEmployee))
                return Forbid("Access denied. You can only view employees from your group.");

            var employeeDto = await _employeeService.GetEmployeeByIdAsync(id);
            return Ok(new { data = employeeDto });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeDto employeeDto)
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null)
                return Unauthorized("Employee not found");

            if (!HasAdminAccess(currentEmployee))
                return Forbid("Access denied. Admin role required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _employeeService.AddEmployeeAsync(employeeDto);
            if (created == null)
                return BadRequest(new { message = "Email already exists" });

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, new
            {
                data = created,
                message = employeeDto.BossId.HasValue ?
                    "Employee created successfully with Boss role (BossId provided)" :
                    "Employee created successfully"
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EmployeeDto employeeDto)
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null)
                return Unauthorized("Employee not found");

            var targetEmployee = await _employeeService.GetEmployeeEntityByIdAsync(id);
            if (targetEmployee == null)
                return NotFound(new { message = "Employee not found" });

            if (!CanModifyEmployeeData(currentEmployee, targetEmployee))
                return Forbid("Access denied. You don't have permission to modify this employee.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _employeeService.UpdateEmployeeAsync(id, employeeDto);
            if (updated == null)
                return BadRequest(new { message = "Email already exists or employee not found" });

            return Ok(new
            {
                data = updated,
                message = employeeDto.BossId.HasValue ?
                    "Employee updated successfully with Boss role (BossId provided)" :
                    "Employee updated successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var currentEmployee = await GetCurrentEmployeeAsync();
            if (currentEmployee == null)
                return Unauthorized("Employee not found");

            if (!HasAdminAccess(currentEmployee))
                return Forbid("Access denied. Admin role required.");

            var deleted = await _employeeService.DeleteEmployeeAsync(id);
            if (!deleted)
                return NotFound(new { message = "Employee not found" });

            return Ok(new { message = "Employee deleted successfully" });
        }

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

        private async Task<Employee?> GetCurrentEmployeeAsync()
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null)
                return null;

            return await _employeeService.GetEmployeeEntityByIdAsync(employeeId.Value);
        }

        private bool HasAdminAccess(Employee employee)
        {
            return employee.Role?.Name == "Boss";
        }

        private bool HasGroupBossAccess(Employee employee)
        {
            var roleName = employee.Role?.Name;
            return roleName != null && roleName.StartsWith("Boss-");
        }

        private bool CanAccessEmployeeData(Employee currentEmployee, Employee targetEmployee)
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

        private bool CanModifyEmployeeData(Employee currentEmployee, Employee targetEmployee)
        {
            if (HasAdminAccess(currentEmployee))
                return true;

            if (currentEmployee.Id == targetEmployee.Id)
                return true;

            if (HasGroupBossAccess(currentEmployee))
            {
                var bossRole = currentEmployee.Role?.Name;
                var employeeRole = targetEmployee.Role?.Name;

                if (string.IsNullOrEmpty(bossRole) || string.IsNullOrEmpty(employeeRole)) return false;

                if (employeeRole.StartsWith("Boss")) return false;

                var roleSuffix = bossRole.Substring("Boss-".Length);
                return employeeRole.EndsWith($"-{roleSuffix}");
            }

            return false;
        }

        #endregion
    }
}


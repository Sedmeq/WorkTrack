using EmployeeAdminPortal.Models.Entities;
using EmployeeAdminPortal.Models.Dto;
using Identity.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Models.Dto;

namespace EmployeeAdminPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<Employee>> Register(RegisterDto request)
        {
            var employee = await authService.RegisterAsync(request);
            if (employee is null)
            {
                return BadRequest("Email already exists");
            }

            var responseEmployee = new
            {
                employee.Id,
                //employee.Name,
                employee.Username,
                employee.Email,
                //employee.Phone,
                //employee.Salary
            };

            return Ok(responseEmployee);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            var token = await authService.LoginAsync(request);
            if (token is null)
            {
                return Unauthorized("Invalid username or password");
            }
            return Ok(token);
        }
    }
}
using DataAccessLayer.Data;
using EmployeeAdminPortal.Models.Entities;
using Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Models.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessLogicLayer.Service
{
    public class AuthService(ApplicationDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<string?> LoginAsync(LoginDto request)
        {
            var employee = await context.Employees.FirstOrDefaultAsync(e => e.Email == request.Email);
            if (employee is null)
            {
                return null;
            }

            if (new PasswordHasher<Employee>().VerifyHashedPassword(employee, employee.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return CreateToken(employee);
        }

        public async Task<Employee?> RegisterAsync(RegisterDto request)
        {
            if (await context.Employees.AnyAsync(e => e.Email == request.Email))
            {
                return null;
            }

            var employee = new Employee
            {
                //Name = request.Name,
                Username = request.Username,
                Email = request.Email,
                //Phone = request.Phone,
                //Salary = request.Salary,
                PasswordHash = ""
            };

            var hashedPassword = new PasswordHasher<Employee>()
                .HashPassword(employee, request.Password);

            employee.PasswordHash = hashedPassword;

            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            return employee;
        }

        private string CreateToken(Employee employee)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, employee.Username),
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
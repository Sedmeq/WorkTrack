using AutoMapper;
using BusinessLogicLayer.Services;
using DataAccessLayer.Data;
using EmployeeAdminPortal.Models.Dto;
using EmployeeAdminPortal.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Models.Dto;

namespace BusinessLogicLayer.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public EmployeeService(ApplicationDbContext context, IRoleService roleService , IMapper mapper)
        {
            _context = context;
            _roleService = roleService;
            _mapper = mapper;
        }

        public async Task<List<EmployeeResponseDto>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Role)
                .Include(e => e.WorkSchedule)
                .Select(e => new EmployeeResponseDto
                {
                    Id = e.Id,
                    Username = e.Username,
                    Email = e.Email,
                    Phone = e.Phone,
                    Salary = e.Salary,
                    RoleId = e.RoleId,
                    RoleName = e.Role != null ? e.Role.Name : null,
                    WorkScheduleId = e.WorkScheduleId,
                    WorkScheduleName = e.WorkSchedule != null ? e.WorkSchedule.Name : null,
                    WorkStartTime = e.WorkSchedule != null ? e.WorkSchedule.StartTime.ToString(@"hh\:mm") : null,
                    WorkEndTime = e.WorkSchedule != null ? e.WorkSchedule.EndTime.ToString(@"hh\:mm") : null,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<List<EmployeeResponseDto>> GetEmployeesByRoleAsync(Guid roleId)
        {
            return await _context.Employees
                .Include(e => e.Role)
                .Include(e => e.WorkSchedule)
                .Where(e => e.RoleId == roleId)
                .Select(e => new EmployeeResponseDto
                {
                    Id = e.Id,
                    Username = e.Username,
                    Email = e.Email,
                    Phone = e.Phone,
                    Salary = e.Salary,
                    RoleId = e.RoleId,
                    RoleName = e.Role != null ? e.Role.Name : null,
                    WorkScheduleId = e.WorkScheduleId,
                    WorkScheduleName = e.WorkSchedule != null ? e.WorkSchedule.Name : null,
                    WorkStartTime = e.WorkSchedule != null ? e.WorkSchedule.StartTime.ToString(@"hh\:mm") : null,
                    WorkEndTime = e.WorkSchedule != null ? e.WorkSchedule.EndTime.ToString(@"hh\:mm") : null,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<List<EmployeeResponseDto>> GetEmployeesByRoleSuffixAsync(string roleSuffix)
        {
            return await _context.Employees
                .Include(e => e.Role)
                .Include(e => e.WorkSchedule)
                .Where(e => e.Role != null && e.Role.Name.EndsWith($"-{roleSuffix}"))
                .Select(e => new EmployeeResponseDto
                {
                    Id = e.Id,
                    Username = e.Username,
                    Email = e.Email,
                    Phone = e.Phone,
                    Salary = e.Salary,
                    RoleId = e.RoleId,
                    RoleName = e.Role != null ? e.Role.Name : null,
                    WorkScheduleId = e.WorkScheduleId,
                    WorkScheduleName = e.WorkSchedule != null ? e.WorkSchedule.Name : null,
                    WorkStartTime = e.WorkSchedule != null ? e.WorkSchedule.StartTime.ToString(@"hh\:mm") : null,
                    WorkEndTime = e.WorkSchedule != null ? e.WorkSchedule.EndTime.ToString(@"hh\:mm") : null,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .ToListAsync();
        }

        //public async Task<List<EmployeeResponseDto>> GetSubordinateEmployeesAsync(Guid bossId)
        //{
        //    var managedRoles = await _context.Roles
        //        .Where(r => r.BossId == bossId)
        //        .Select(r => r.Id)
        //        .ToListAsync();

        //    return await _context.Employees
        //        .Include(e => e.Role)
        //        .Include(e => e.WorkSchedule)
        //        .Where(e => e.RoleId.HasValue && managedRoles.Contains(e.RoleId.Value))
        //        .Select(e => new EmployeeResponseDto
        //        {
        //            Id = e.Id,
        //            Username = e.Username,
        //            Email = e.Email,
        //            Phone = e.Phone,
        //            Salary = e.Salary,
        //            RoleId = e.RoleId,
        //            RoleName = e.Role != null ? e.Role.Name : null,
        //            WorkScheduleId = e.WorkScheduleId,
        //            WorkScheduleName = e.WorkSchedule != null ? e.WorkSchedule.Name : null,
        //            WorkStartTime = e.WorkSchedule != null ? e.WorkSchedule.StartTime.ToString(@"hh\:mm") : null,
        //            WorkEndTime = e.WorkSchedule != null ? e.WorkSchedule.EndTime.ToString(@"hh\:mm") : null,
        //            CreatedAt = e.CreatedAt,
        //            UpdatedAt = e.UpdatedAt
        //        })
        //        .ToListAsync();
        //}

        public async Task<EmployeeResponseDto?> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _context.Employees
                .Include(e => e.Role)
                .Include(e => e.WorkSchedule)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return null;

            return new EmployeeResponseDto
            {
                Id = employee.Id,
                Username = employee.Username,
                Email = employee.Email,
                Phone = employee.Phone,
                Salary = employee.Salary,
                RoleId = employee.RoleId,
                RoleName = employee.Role?.Name,
                WorkScheduleId = employee.WorkScheduleId,
                WorkScheduleName = employee.WorkSchedule?.Name,
                WorkStartTime = employee.WorkSchedule?.StartTime.ToString(@"hh\:mm"),
                WorkEndTime = employee.WorkSchedule?.EndTime.ToString(@"hh\:mm"),
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt
            };
        }

        public async Task<EmployeeResponseDto?> AddEmployeeAsync(EmployeeDto employeeDto)
        {
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == employeeDto.Email);

            if (existingEmployee != null)
                return null;

            // Role ID müəyyən et: əgər bossId verilmişsə, Boss rolu veriləcək
            var finalRoleId = await _roleService.DetermineRoleIdAsync(employeeDto.RoleId, employeeDto.BossId);

            var employee = new Employee
            {
                Username = employeeDto.Username,
                Email = employeeDto.Email,
                Phone = employeeDto.Phone,
                Salary = employeeDto.Salary,
                RoleId = finalRoleId,
                WorkScheduleId = employeeDto.WorkScheduleId,
                PasswordHash = "",
                CreatedAt = DateTime.Now
            };

            var hashedPassword = new PasswordHasher<Employee>()
                .HashPassword(employee, employeeDto.Password);

            employee.PasswordHash = hashedPassword;

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return await GetEmployeeByIdAsync(employee.Id);
        }

        public async Task<EmployeeResponseDto?> UpdateEmployeeAsync(Guid id, EmployeeDto updatedEmployeeDto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return null;

            if (employee.Email != updatedEmployeeDto.Email)
            {
                var existingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Email == updatedEmployeeDto.Email && e.Id != id);
                if (existingEmployee != null)
                    return null;
            }

            // Role ID müəyyən et: əgər bossId verilmişsə, Boss rolu veriləcək
            var finalRoleId = await _roleService.DetermineRoleIdAsync(updatedEmployeeDto.RoleId, updatedEmployeeDto.BossId);

            employee.Username = updatedEmployeeDto.Username;
            employee.Email = updatedEmployeeDto.Email;
            employee.Phone = updatedEmployeeDto.Phone;
            employee.Salary = updatedEmployeeDto.Salary;
            employee.RoleId = finalRoleId;
            employee.WorkScheduleId = updatedEmployeeDto.WorkScheduleId;
            employee.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrEmpty(updatedEmployeeDto.Password))
            {
                var hashedPassword = new PasswordHasher<Employee>()
                    .HashPassword(employee, updatedEmployeeDto.Password);
                employee.PasswordHash = hashedPassword;
            }

            await _context.SaveChangesAsync();
            return await GetEmployeeByIdAsync(employee.Id);
        }

        public async Task<bool> DeleteEmployeeAsync(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return false;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Employee?> GetEmployeeEntityByIdAsync(Guid id)
        {
            return await _context.Employees
                .Include(e => e.Role)
                .Include(e => e.WorkSchedule)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // Role məlumatları üçün helper methods
        public async Task<List<Models.Models.Entities.Role>> GetAvailableRolesAsync()
        {
            return await _roleService.GetAvailableRolesAsync();
        }
    }
}

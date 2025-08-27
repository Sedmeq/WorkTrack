using AutoMapper;
using BusinessLogicLayer.Services;
using DataAccessLayer.Data;
using EmployeeAdminPortal.Models.Dto;
using EmployeeAdminPortal.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Model.Dto;
using Models.Models.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public EmployeeService(ApplicationDbContext context, IRoleService roleService, IMapper mapper)
        {
            _context = context;
            _roleService = roleService;
            _mapper = mapper;
        }

        // yeni elave ediulen mothod !!!
        public async Task<VacationBalanceDto?> GetVacationBalanceAsync(Guid employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return null;

            var daysWorked = (DateTime.UtcNow - employee.CreatedAt).TotalDays;
            var totalAccruedDays = 30 * (daysWorked / 365.0);

            var vacationsTaken = await _context.Vacations
                .Where(v => v.EmployeeId == employeeId && v.Status == "Approved")
                .ToListAsync();

            double daysTaken = vacationsTaken.Sum(v => (v.EndDate - v.StartDate).TotalDays + 1);

            return new VacationBalanceDto
            {
                TotalAccruedDays = Math.Round(totalAccruedDays, 1),
                DaysTaken = Math.Round(daysTaken, 1),
                RemainingDays = Math.Round(totalAccruedDays - daysTaken, 1)
            };
        }
        public async Task<EmployeeResponseDto?> AddEmployeeAsync(EmployeeDto employeeDto)
        {
            if (await _context.Employees.AnyAsync(e => e.Email == employeeDto.Email))
                return null;

            if (employeeDto.BossId.HasValue && !await _context.Employees.AnyAsync(e => e.Id == employeeDto.BossId.Value))
                return null;

            var employee = _mapper.Map<Employee>(employeeDto);

            employee.RoleId = await _roleService.DetermineRoleIdAsync(employeeDto.RoleId, employeeDto.BossId);
            employee.CreatedAt = DateTime.UtcNow;
            employee.PasswordHash = new PasswordHasher<Employee>().HashPassword(employee, employeeDto.Password);

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return await GetEmployeeByIdAsync(employee.Id);
        }



        public async Task<EmployeeResponseDto?> UpdateEmployeeAsync(Guid id, EmployeeDto updatedEmployeeDto)
        {
            // 1. Mövcud işçini bazadan tapırıq (izləmə aktiv qalır)
            var employeeToUpdate = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (employeeToUpdate == null)
            {
                return null;
            }

            // Email-in başqası tərəfindən istifadə olunmadığını yoxlayırıq
            if (employeeToUpdate.Email != updatedEmployeeDto.Email && await _context.Employees.AnyAsync(e => e.Email == updatedEmployeeDto.Email && e.Id != id))
            {
                return null;
            }

            // BossId-nin mövcud olduğunu yoxlayırıq
            if (updatedEmployeeDto.BossId.HasValue && !await _context.Employees.AnyAsync(e => e.Id == updatedEmployeeDto.BossId.Value))
            {
                return null;
            }

            // Özü özünün rəhbəri ola bilməz
            if (updatedEmployeeDto.BossId.HasValue && id == updatedEmployeeDto.BossId.Value)
            {
                return null;
            }

            // 2. Yeni obyekt yaratmaq əvəzinə, AutoMapper ilə mövcud obyekti yeniləyirik
            _mapper.Map(updatedEmployeeDto, employeeToUpdate);

            // Role və digər sahələri təyin edirik
            employeeToUpdate.RoleId = await _roleService.DetermineRoleIdAsync(updatedEmployeeDto.RoleId, updatedEmployeeDto.BossId);
            employeeToUpdate.UpdatedAt = DateTime.UtcNow;

            // Şifrə yenilənibsə, hash-i dəyişirik
            if (!string.IsNullOrEmpty(updatedEmployeeDto.Password))
            {
                employeeToUpdate.PasswordHash = new PasswordHasher<Employee>().HashPassword(employeeToUpdate, updatedEmployeeDto.Password);
            }

            // 3. Sadəcə dəyişiklikləri yaddaşa veririk. EF hansı sahələrin dəyişdiyini özü bilir.
            await _context.SaveChangesAsync();
            return await GetEmployeeByIdAsync(id);
        }

        // --- Dəyişdirilməmiş digər metodlar ---

        public async Task<List<EmployeeResponseDto>> GetAllEmployeesAsync()
        {
            var employees = await _context.Employees.Include(e => e.Role).Include(e => e.WorkSchedule).ToListAsync();
            return _mapper.Map<List<EmployeeResponseDto>>(employees);
        }

        public async Task<List<EmployeeResponseDto>> GetEmployeesByRoleAsync(Guid roleId)
        {
            var employees = await _context.Employees.Include(e => e.Role).Include(e => e.WorkSchedule).Where(e => e.RoleId == roleId).ToListAsync();
            return _mapper.Map<List<EmployeeResponseDto>>(employees);
        }

        public async Task<List<EmployeeResponseDto>> GetSubordinateEmployeesAsync(Guid bossId)
        {
            var employees = await _context.Employees.Include(e => e.Role).Include(e => e.WorkSchedule).Where(e => e.BossId == bossId).ToListAsync();
            return _mapper.Map<List<EmployeeResponseDto>>(employees);
        }

        public async Task<EmployeeResponseDto?> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _context.Employees.Include(e => e.Role).Include(e => e.WorkSchedule).FirstOrDefaultAsync(e => e.Id == id);
            return _mapper.Map<EmployeeResponseDto>(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Employee?> GetEmployeeEntityByIdAsync(Guid id)
        {
            return await _context.Employees.Include(e => e.Role).Include(e => e.WorkSchedule).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Models.Models.Entities.Role>> GetAvailableRolesAsync()
        {
            return await _roleService.GetAvailableRolesAsync();
        }
    }
}
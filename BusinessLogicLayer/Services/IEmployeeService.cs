using EmployeeAdminPortal.Models.Dto;
using EmployeeAdminPortal.Models.Entities;
using Models.Model.Dto;
using Models.Models.Dto;
using Models.Models.Entities;

namespace BusinessLogicLayer.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeResponseDto>> GetAllEmployeesAsync();
        Task<List<EmployeeResponseDto>> GetEmployeesByRoleAsync(Guid roleId);
        Task<List<EmployeeResponseDto>> GetSubordinateEmployeesAsync(Guid bossId); // YENİ
        Task<EmployeeResponseDto?> GetEmployeeByIdAsync(Guid id);
        Task<EmployeeResponseDto?> AddEmployeeAsync(EmployeeDto employeeDto);
        Task<EmployeeResponseDto?> UpdateEmployeeAsync(Guid id, EmployeeDto updatedEmployee);
        Task<bool> DeleteEmployeeAsync(Guid id);
        Task<Employee?> GetEmployeeEntityByIdAsync(Guid id);
        Task<List<Role>> GetAvailableRolesAsync();

        Task<VacationBalanceDto?> GetVacationBalanceAsync(Guid employeeId); // Bu sətri əlavə edin

    }
}
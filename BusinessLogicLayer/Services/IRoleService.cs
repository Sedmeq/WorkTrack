using Models.Models.Dto;
using Models.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public interface IRoleService
    {
        Task<List<Role>> GetAvailableRolesAsync();
        Task<Role?> GetRoleByIdAsync(Guid roleId);
        Task<Role?> GetBossRoleAsync();
        Task<Role?> GetEmployeeRoleAsync();
        Task<List<Role>> GetDepartmentBossRolesAsync();
        Task<Guid?> DetermineRoleIdAsync(Guid? selectedRoleId, Guid? bossId);
    }
}

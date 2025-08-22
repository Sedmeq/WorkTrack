using BusinessLogicLayer.Services;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using Models.Models.Dto;
using Models.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAvailableRolesAsync()
        {
            return await _context.Roles
                .Where(r => r.Name == "Employee" || r.Name.StartsWith("Boss-") || r.Name == "Boss")
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<Role?> GetRoleByIdAsync(Guid roleId)
        {
            return await _context.Roles.FindAsync(roleId);
        }

        public async Task<Role?> GetBossRoleAsync()
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Boss");
        }

        public async Task<Role?> GetEmployeeRoleAsync()
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Employee");
        }

        public async Task<List<Role>> GetDepartmentBossRolesAsync()
        {
            return await _context.Roles
                .Where(r => r.Name.StartsWith("Boss-"))
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Role ID müəyyən edir: əgər bossId verilmişsə, Boss rolu veriləcək
        /// </summary>
        public async Task<Guid?> DetermineRoleIdAsync(Guid? selectedRoleId, Guid? bossId)
        {
            // Əgər bossId verilmişsə, avtomatik Boss rolu veriləcək
            if (bossId.HasValue)
            {
                var bossRole = await GetBossRoleAsync();
                return bossRole?.Id;
            }

            // Əks halda, seçilmiş role ID qayıdılacaq
            return selectedRoleId;
        }
    }
}

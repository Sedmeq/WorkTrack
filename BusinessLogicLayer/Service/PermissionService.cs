using AutoMapper;
using BusinessLogicLayer.Services;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using Models.Model.Dto;
using Models.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PermissionService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PermissionDto?> SubmitPermissionAsync(Guid employeeId, CreatePermissionDto requestDto)
        {
            // İşçinin BossId-ni tapırıq
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee == null) return null;

            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                BossId = employee.BossId,
                StartDate = requestDto.StartDate,
                EndDate = requestDto.EndDate,
                Reason = requestDto.Reason,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            return _mapper.Map<PermissionDto>(permission);
        }

        public async Task<bool> ApprovePermissionAsync(Guid requestId, Guid bossId)
        {
            var Permission = await _context.Permissions
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == "Pending");

            if (Permission == null) return false;

            // Yalnız müraciəti qəbul edən boss həmin işçinin rəhbəridirsə təsdiqləsin
            if (Permission.Employee?.BossId != bossId)
            {
                return false;
            }

            Permission.Status = "Approved";
            _context.Permissions.Update(Permission);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DenyPermissionAsync(Guid requestId, Guid bossId)
        {
            var Permission = await _context.Permissions
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == "Pending");

            if (Permission == null) return false;

            // Yalnız müraciəti rədd edən boss həmin işçinin rəhbəridirsə rədd etsin
            if (Permission.Employee?.BossId != bossId)
            {
                return false;
            }

            Permission.Status = "Denied";
            _context.Permissions.Update(Permission);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PermissionDto?> GrantLeaveAsync(Guid bossId, BossPermissionDto grantDto)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == grantDto.EmployeeId);
            if (employee == null || employee.BossId != bossId) return null;

            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                EmployeeId = grantDto.EmployeeId,
                BossId = bossId,
                StartDate = grantDto.StartDate,
                EndDate = grantDto.EndDate,
                Reason = grantDto.Reason,
                Status = "Approved",
                CreatedAt = DateTime.UtcNow
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            return _mapper.Map<PermissionDto>(permission);
        }

        public async Task<List<PermissionDto>> GetMyPermissionsAsync(Guid employeeId)
        {
            var Permissions = await _context.Permissions
                .Where(r => r.EmployeeId == employeeId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<PermissionDto>>(Permissions);
        }

        public async Task<List<PermissionDto>> GetPendingPermissionsForBossAsync(Guid bossId)
        {
            var Permissions = await _context.Permissions
                .Include(r => r.Employee)
                .Where(r => r.BossId == bossId && r.Status == "Pending")
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<PermissionDto>>(Permissions);
        }
    }
}

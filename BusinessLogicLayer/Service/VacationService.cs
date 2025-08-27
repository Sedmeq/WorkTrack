using AutoMapper;
using BusinessLogicLayer.Services;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using Models.Model.Dto;
using Models.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Service
{
    public class VacationService : IVacationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;

        public VacationService(ApplicationDbContext context, IMapper mapper, IEmployeeService employeeService)
        {
            _context = context;
            _mapper = mapper;
            _employeeService = employeeService;
        }

        public async Task<VacationDto?> SubmitVacationAsync(Guid employeeId, CreateVacationDto requestDto)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null || employee.BossId == null) return null;

            var balance = await _employeeService.GetVacationBalanceAsync(employeeId);
            if (balance == null) return null;

            var requestedDays = (requestDto.EndDate - requestDto.StartDate).TotalDays + 1;
            if (requestedDays <= 0 || requestedDays > balance.RemainingDays) return null;

            var vacation = new Vacation
            {
                EmployeeId = employeeId,
                BossId = employee.BossId,
                StartDate = requestDto.StartDate,
                EndDate = requestDto.EndDate,
                Reason = requestDto.Reason,
                Status = "Pending"
            };

            _context.Vacations.Add(vacation);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<VacationDto>(vacation);
            dto.EmployeeName = employee.Username;
            return dto;
        }

        public async Task<bool> ApproveVacationAsync(Guid requestId, Guid bossId)
        {
            var vacation = await _context.Vacations.FirstOrDefaultAsync(v => v.Id == requestId && v.BossId == bossId && v.Status == "Pending");
            if (vacation == null) return false;
            vacation.Status = "Approved";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DenyVacationAsync(Guid requestId, Guid bossId)
        {
            var vacation = await _context.Vacations.FirstOrDefaultAsync(v => v.Id == requestId && v.BossId == bossId && v.Status == "Pending");
            if (vacation == null) return false;
            vacation.Status = "Denied";
            await _context.SaveChangesAsync();
            return true;
        }

        //public async Task<VacationDto?> GrantVacationAsync(Guid bossId, BossVacationDto grantDto)
        //{
        //    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == grantDto.EmployeeId && e.BossId == bossId);
        //    if (employee == null) return null;
        //    // Grant edərkən də balans yoxlanıla bilər (opsional)

        //    var vacation = new Vacation
        //    {
        //        EmployeeId = grantDto.EmployeeId,
        //        BossId = bossId,
        //        StartDate = grantDto.StartDate,
        //        EndDate = grantDto.EndDate,
        //        Reason = grantDto.Reason,
        //        Status = "Approved"
        //    };
        //    _context.Vacations.Add(vacation);
        //    await _context.SaveChangesAsync();
        //    return _mapper.Map<VacationDto>(vacation);
        //}

        public async Task<List<VacationDto>> GetMyVacationsAsync(Guid employeeId)
        {
            return _mapper.Map<List<VacationDto>>(await _context.Vacations
                .Where(v => v.EmployeeId == employeeId)
                .OrderByDescending(v => v.CreatedAt).ToListAsync());
        }

        public async Task<List<VacationDto>> GetPendingVacationsForBossAsync(Guid bossId)
        {
            return _mapper.Map<List<VacationDto>>(await _context.Vacations
                .Include(v => v.Employee)
                .Where(v => v.BossId == bossId && v.Status == "Pending")
                .OrderByDescending(v => v.CreatedAt).ToListAsync());
        }
    }
}
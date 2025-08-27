using Models.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public interface IVacationService
    {
        Task<VacationDto?> SubmitVacationAsync(Guid employeeId, CreateVacationDto requestDto);
        Task<bool> ApproveVacationAsync(Guid requestId, Guid bossId);
        Task<bool> DenyVacationAsync(Guid requestId, Guid bossId);
       // Task<VacationDto?> GrantVacationAsync(Guid bossId, BossVacationDto grantDto);
        Task<List<VacationDto>> GetMyVacationsAsync(Guid employeeId);
        Task<List<VacationDto>> GetPendingVacationsForBossAsync(Guid bossId);
    }
}

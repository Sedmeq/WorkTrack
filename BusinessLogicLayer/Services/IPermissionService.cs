using Models.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public interface IPermissionService
    {
        Task<PermissionDto?> SubmitPermissionAsync(Guid employeeId, CreatePermissionDto requestDto);

        // Rəhbər tərəfindən icazə müraciətinin təsdiqi
        Task<bool> ApprovePermissionAsync(Guid requestId, Guid bossId);

        // Rəhbər tərəfindən icazə müraciətinin rədd edilməsi
        Task<bool> DenyPermissionAsync(Guid requestId, Guid bossId);

        // Rəhbər tərəfindən işçi üçün birbaşa icazə verilməsi
        Task<PermissionDto?> GrantLeaveAsync(Guid bossId, BossPermissionDto grantDto);

        // İşçinin öz məzuniyyət müraciətlərini görməsi
        Task<List<PermissionDto>> GetMyPermissionsAsync(Guid employeeId);

        // Rəhbərin təsdiqləməsi lazım olan müraciətləri görməsi
        Task<List<PermissionDto>> GetPendingPermissionsForBossAsync(Guid bossId);
    }
}

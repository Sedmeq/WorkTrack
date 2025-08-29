using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Dto
{
    public class EmployeeResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public decimal Salary { get; set; }

        // Department hissələri SİLİNDİ
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
        public Guid? WorkScheduleId { get; set; }
        public string? WorkScheduleName { get; set; }
        public string? WorkStartTime { get; set; }
        public string? WorkEndTime { get; set; }
        public Guid? BossId { get; set; }
        public string? BossName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

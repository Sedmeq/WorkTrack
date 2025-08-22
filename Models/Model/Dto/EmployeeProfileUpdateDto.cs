using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Dto
{
    public class EmployeeProfileUpdateDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public decimal Salary { get; set; }

        /// <summary>
        /// Employee öz rolunu seçə bilər: Employee, Boss-IT, Boss-Marketing, Boss-Finance, Boss-HR, Boss-Sales, Boss-Operations
        /// </summary>
        public Guid? RoleId { get; set; }

        public Guid? WorkScheduleId { get; set; }

        /// <summary>
        /// Əgər bu dəyər verilsə, avtomatik Boss rolu veriləcək və bu parameter üstünlük təşkil edəcək
        /// </summary>
        public Guid? BossId { get; set; }
    }
}

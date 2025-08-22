using EmployeeAdminPortal.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Entities
{
    public class EmployeeTimeLog
    {
        public Guid Id { get; set; }

        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        public DateTime CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public TimeSpan? WorkDuration { get; set; }

        public string? Notes { get; set; }

        // Navigation property
        public Employee Employee { get; set; }

        public bool IsCheckedOut => CheckOutTime.HasValue;

        public void CalculateWorkDuration()
        {
            if (CheckOutTime.HasValue)
            {
                WorkDuration = CheckOutTime.Value - CheckInTime;
            }
        }
    }
}

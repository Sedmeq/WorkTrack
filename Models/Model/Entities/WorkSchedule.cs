using EmployeeAdminPortal.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Entities
{
    public class WorkSchedule
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// Gün ərzində neçə saat işləmək lazımdır (məs: 8 saat)
        /// </summary>
        public int RequiredWorkHours { get; set; }

        /// <summary>
        /// Günlük minimum işləmə vaxtı (dəqiqələrlə)
        /// </summary>
        public int MinimumWorkMinutes { get; set; }

        /// <summary>
        /// Maksimum gecikme müddəti (dəqiqələrlə)
        /// </summary>
        public int MaxLatenessMinutes { get; set; } = 15;

        /// <summary>
        /// Maksimum erkən gedişə icazə (dəqiqələrlə)
        /// </summary>
        //public int MaxEarlyLeaveMinutes { get; set; } = 15;

        //public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

        // Helper methods
        public bool IsWithinWorkHours(TimeSpan time)
        {
            if (StartTime <= EndTime)
            {
                // Normal shift (e.g., 8:00-17:00)
                return time >= StartTime && time <= EndTime;
            }
            else
            {
                // Night shift (e.g., 22:00-06:00)
                return time >= StartTime || time <= EndTime;
            }
        }

        //public bool IsLate(TimeSpan checkInTime)
        //{
        //    var allowedStartTime = StartTime.Add(TimeSpan.FromMinutes(MaxLatenessMinutes));
        //    return checkInTime > allowedStartTime;
        //}

        //public bool IsEarlyLeave(TimeSpan checkOutTime)
        //{
        //    var minimumEndTime = EndTime.Subtract(TimeSpan.FromMinutes(MaxEarlyLeaveMinutes));
        //    return checkOutTime < minimumEndTime;
        //}

        //public TimeSpan GetLatenessTime(TimeSpan checkInTime)
        //{
        //    if (checkInTime > StartTime)
        //    {
        //        return checkInTime - StartTime;
        //    }
        //    return TimeSpan.Zero;
        //}

        //public TimeSpan GetEarlyLeaveTime(TimeSpan checkOutTime)
        //{
        //    if (checkOutTime < EndTime)
        //    {
        //        return EndTime - checkOutTime;
        //    }
        //    return TimeSpan.Zero;
        //}
    }
}

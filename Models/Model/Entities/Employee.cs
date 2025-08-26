using Models.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace EmployeeAdminPortal.Models.Entities
{
    public class Employee
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public Guid? RoleId { get; set; }
        public Guid? WorkScheduleId { get; set; }

        // YENİ: Employee-in birbaşa boss-unu təyin etmək üçün
        public Guid? BossId { get; set; }

        public DateTime CreatedAt { get; set; } //= DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual Role? Role { get; set; }
        public virtual WorkSchedule? WorkSchedule { get; set; }
        public virtual ICollection<EmployeeTimeLog> TimeLogs { get; set; } = new List<EmployeeTimeLog>();

        // Bu role-a boss kimi təyin edilmiş role-lar
        //public virtual ICollection<Role> ManagedRoles { get; set; } = new List<Role>();

        // YENİ: Employee-in boss-u
        public virtual Employee? Boss { get; set; }

        // YENİ: Bu employee-in subordinate-ları
        public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();

        //// Helper methods
        public bool IsBoss()
        {
            return Role?.Name == "Boss" || (Role?.Name?.StartsWith("Boss-") ?? false);
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace EmployeeAdminPortal.Models.Dto
{
    public class EmployeeDto
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }

        public Guid? RoleId { get; set; }
        public Guid? WorkScheduleId { get; set; }

        /// <summary>
        /// Boss ID qeyd edilərsə, avtomatik Boss rolu veriləcək
        /// </summary>
        public Guid? BossId { get; set; }
    }
}
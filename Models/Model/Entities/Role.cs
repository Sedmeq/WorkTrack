using EmployeeAdminPortal.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(200)]
        public string? Description { get; set; }
        //public Guid? BossId { get; set; }
        //public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        //public virtual Employee? Boss { get; set; }
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}

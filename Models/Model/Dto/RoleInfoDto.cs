using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Dto
{
    public class RoleInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsBoss { get; set; }
        public bool IsDepartmentBoss { get; set; }
        public string? Department { get; set; } // Boss-IT -> IT, Boss-Marketing -> Marketing
    }
}

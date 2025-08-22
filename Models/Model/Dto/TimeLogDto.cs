using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Models.Dto
{
    public class TimeLogDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;

        // DepartmentName SİLİNDİ - artıq department yoxdur
        public string? RoleName { get; set; }

        [JsonPropertyName("checkInTime")]
        public string CheckInTime { get; set; } = string.Empty;

        [JsonPropertyName("checkOutTime")]
        public string? CheckOutTime { get; set; }

        [JsonPropertyName("workDuration")]
        public string? WorkDuration { get; set; }

        [JsonPropertyName("workDurationInMinutes")]
        public int? WorkDurationInMinutes { get; set; }

        public string? Notes { get; set; }
        public bool IsCheckedOut { get; set; }

        // Raw DateTime values for internal use
        [JsonIgnore]
        public DateTime CheckInTimeRaw { get; set; }

        [JsonIgnore]
        public DateTime? CheckOutTimeRaw { get; set; }

        [JsonIgnore]
        public TimeSpan? WorkDurationRaw { get; set; }
    }
}

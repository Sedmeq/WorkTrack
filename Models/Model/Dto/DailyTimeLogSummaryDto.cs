using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Models.Dto
{
    public class DailyTimeLogSummaryDto
    {
        public string Date { get; set; } = string.Empty;
        public string TotalWorkTime { get; set; } = string.Empty;
        public int CheckInCount { get; set; }
        public List<TimeLogDto> TimeLogs { get; set; } = new List<TimeLogDto>();

        [JsonIgnore]
        public DateTime DateRaw { get; set; }

        [JsonIgnore]
        public TimeSpan TotalWorkTimeRaw { get; set; }
    }
}

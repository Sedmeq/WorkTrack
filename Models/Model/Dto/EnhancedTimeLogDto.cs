using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Dto
{
    public class EnhancedTimeLogDto : TimeLogDto
    {
        public string? WorkScheduleName { get; set; }
        public string? ExpectedStartTime { get; set; }
        public string? ExpectedEndTime { get; set; }
        public string? LatenessTime { get; set; }
        public string? EarlyLeaveTime { get; set; }
        public bool IsLate { get; set; }
        public bool IsEarlyLeave { get; set; }
        public bool IsWithinSchedule { get; set; }
        public string? WorkEfficiency { get; set; } // İşlənmiş vaxt / gözlənilən vaxt
    }
}
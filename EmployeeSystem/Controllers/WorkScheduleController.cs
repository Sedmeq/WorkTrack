using BusinessLogicLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Data;
using Models.Model.Entities;

namespace EmployeeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkScheduleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var workSchedules = await _context.WorkSchedules
                .Select(ws => new
                {
                    id = ws.Id,
                    name = ws.Name,
                    description = ws.Description,
                    startTime = ws.StartTime.ToString(@"hh\:mm"),
                    endTime = ws.EndTime.ToString(@"hh\:mm"),
                    isActive = ws.IsActive
                })
                .Where(ws => ws.isActive)
                .ToListAsync();

            return Ok(new { data = workSchedules, count = workSchedules.Count });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var workSchedule = await _context.WorkSchedules
                .Where(ws => ws.Id == id && ws.IsActive)
                .Select(ws => new
                {
                    id = ws.Id,
                    name = ws.Name,
                    description = ws.Description,
                    startTime = ws.StartTime.ToString(@"hh\:mm"),
                    endTime = ws.EndTime.ToString(@"hh\:mm"),
                    isActive = ws.IsActive
                })
                .FirstOrDefaultAsync();

            if (workSchedule == null)
                return NotFound(new { message = "Work schedule not found" });

            return Ok(new { data = workSchedule });
        }
    }
}

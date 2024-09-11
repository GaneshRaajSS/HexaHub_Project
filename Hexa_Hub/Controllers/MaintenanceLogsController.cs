using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hexa_Hub.Interface;
using Hexa_Hub.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hexa_Hub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceLogsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMaintenanceLogRepo _maintenanceLogRepo;
        public MaintenanceLogsController(DataContext context, IMaintenanceLogRepo maintenanceLogRepo)
        {
            _context = context;
            _maintenanceLogRepo = maintenanceLogRepo;
        }

        // GET: api/MaintenanceLogs
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MaintenanceLog>>> GetMaintenanceLogs()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole == "Admin")
            {
                return await _maintenanceLogRepo.GetAllMaintenanceLog();
            }
            else
            {
                var req = await _maintenanceLogRepo.GetMaintenanceLogByUserId(userId);
                if (req == null)
                {
                    return NotFound();
                }
                return Ok(req);
            }
        }

        // GET: api/MaintenanceLogs/5
        [HttpGet("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<MaintenanceLog>> GetMaintenanceLog(int id)
        {

            var maintenanceLog = await _maintenanceLogRepo.GetMaintenanceLogById(id);

            if (maintenanceLog == null)
            {
                return NotFound();
            }

            return maintenanceLog;
        }

        // PUT: api/MaintenanceLogs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutMaintenanceLog(int id, MaintenanceLog maintenanceLog)
        {
            if (id != maintenanceLog.MaintenanceId)
            {
                return BadRequest();
            }
            _maintenanceLogRepo.UpdateMaintenanceLog(maintenanceLog);

            try
            {
                await _maintenanceLogRepo.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaintenanceLogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //// POST: api/MaintenanceLogs
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public async Task<ActionResult<MaintenanceLog>> PostMaintenanceLog(MaintenanceLog maintenanceLog)
        //{
        //    _maintenanceLogRepo.AddMaintenanceLog(maintenanceLog);
        //    await _maintenanceLogRepo.Save();

        //    return CreatedAtAction("GetMaintenanceLog", new { id = maintenanceLog.MaintenanceId }, maintenanceLog);
        //}

        // DELETE: api/MaintenanceLogs/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMaintenanceLog(int id)
        {
            try
            {
                await _maintenanceLogRepo.DeleteMaintenanceLog(id);
                await _maintenanceLogRepo.Save();
                return NoContent();
            }
            catch (Exception)
            {
                if (id == null)
                    return NotFound();
                return BadRequest();
            }
        }

        private bool MaintenanceLogExists(int id)
        {
            return _context.MaintenanceLogs.Any(e => e.MaintenanceId == id);
        }
    }
}

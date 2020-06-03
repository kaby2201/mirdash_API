using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MissionQueuesResponseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MissionQueuesResponseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MissionQueuesResponse
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MissionQueuesResponse>>> GetMissionQueuesResponse()
        => await _context.MissionQueuesResponse.Include(r => r.Robot).ToListAsync();
        
        
        // GET: api/MissionQueuesResponse/5
        [HttpGet("robot={id}")]
        public async Task<List<MissionQueuesResponse>> GetMissionQueuesResponseByRobot(int id)
        {
            var responses = await _context.MissionQueuesResponse
                .Where(r => r.RobotId == id)
                .OrderByDescending(o => o.Id)
                .ToListAsync();
            
            if (responses == null)
                return null;
            

            return responses;
        }
        

        // GET: api/MissionQueuesResponse/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MissionQueuesResponse>> GetMissionQueuesResponse(int id)
        {
            var missionQueuesResponse = await _context.MissionQueuesResponse.FindAsync(id);

            if (missionQueuesResponse == null)
                return NotFound();

            return missionQueuesResponse;
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMissionQueuesResponse(int id, MissionQueuesResponse missionQueuesResponse)
        {
            if (id != missionQueuesResponse.Id) return BadRequest();
            
            _context.Entry(missionQueuesResponse).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MissionQueuesResponseExists(id))
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
        

        private bool MissionQueuesResponseExists(int id)
        => _context.MissionQueuesResponse.Any(e => e.Id == id);
        
    }
}

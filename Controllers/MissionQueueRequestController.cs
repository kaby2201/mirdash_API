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
    public class MissionQueueRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MissionQueueRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MissionQueueRequest
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MissionQueueRequest>>> GetMissionQueueRequests()
        => await _context.MissionQueueRequests.Include(r => r.RobotId).ToListAsync();
        

        // GET: api/MissionQueueRequest/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MissionQueueRequest>> GetMissionQueueRequest(int id)
        {
            var missionQueueRequest = await _context.MissionQueueRequests.FindAsync(id);

            if (missionQueueRequest == null)
            {
                return NotFound();
            }

            return missionQueueRequest;
        }


        // POST: api/MissionQueueRequest
        [HttpPost("robotId={id}")]
        public async Task<ActionResult<MissionQueueRequest>> PostMissionQueueRequest(MissionQueueRequest missionQueueRequest, int id)
        {
            missionQueueRequest.RobotId = id;
            _context.MissionQueueRequests.Add(missionQueueRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMissionQueueRequest", new { id = missionQueueRequest.Id }, missionQueueRequest);
        }

        // DELETE: api/MissionQueueRequest/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MissionQueueRequest>> DeleteMissionQueueRequest(int id)
        {
            var missionQueueRequest = await _context.MissionQueueRequests.FindAsync(id);
            if (missionQueueRequest == null)
            {
                return NotFound();
            }

            _context.MissionQueueRequests.Remove(missionQueueRequest);
            await _context.SaveChangesAsync();

            return missionQueueRequest;
        }

        private bool MissionQueueRequestExists(int id)
        => _context.MissionQueueRequests.Any(e => e.Id == id);
        
    }
}

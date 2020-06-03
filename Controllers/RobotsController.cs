using System.Linq;
using backend.Data;
using backend.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RobotsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RobotsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Robots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Robot>>> GetRobots() =>
            await _context.Robots.OrderByDescending(o => o.Id).ToListAsync();
        

        // GET: /Robots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Robot>> GetRobot(int id)
        {
            var robot = await _context.Robots.FindAsync(id);

            if (robot == null)
                return NotFound();

            return robot;
        }

        // PUT: /Robots/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRobot(int id, Robot robot)
        {
            if (id != robot.Id)
            {
                return BadRequest();
            }

            _context.Entry(robot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RobotExists(id))
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

        // POST: /Robots
        [HttpPost]
        public async Task<ActionResult<Robot>> PostRobot(Robot robot)
        {
            _context.Robots.Add(robot);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRobot", new { id = robot.Id }, robot);
        }

        // DELETE: /Robots/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Robot>> DeleteRobot(int id)
        {
            var robot = await _context.Robots.FindAsync(id);
            if (robot == null)
                return NotFound();
            

            _context.Robots.Remove(robot);
            await _context.SaveChangesAsync();

            return robot;
        }

        private bool RobotExists(int id) => _context.Robots.Any(e => e.Id == id);
        
    }
}

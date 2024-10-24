using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlgebricEquationSystemSolver.DataAccess;
using AlgebricEquationSystemSolver.DataAccess.Models;

namespace AlgebricEquationSystemSolver.WEBApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlgebricEquationSystemsController : ControllerBase
    {
        private readonly AlgebricEquationSystemDbContext _context;

        public AlgebricEquationSystemsController(AlgebricEquationSystemDbContext context)
        {
            _context = context;
        }

        // GET: api/AlgebricEquationSystems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlgebricEquationSystem>>> GetEquations()
        {
            return await _context.Equations.ToListAsync();
        }

        // GET: api/AlgebricEquationSystems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AlgebricEquationSystem>> GetAlgebricEquationSystem(Guid id)
        {
            var algebricEquationSystem = await _context.Equations.FindAsync(id);

            if (algebricEquationSystem == null)
            {
                return NotFound();
            }

            return algebricEquationSystem;
        }

        // PUT: api/AlgebricEquationSystems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlgebricEquationSystem(Guid id, AlgebricEquationSystem algebricEquationSystem)
        {
            if (id != algebricEquationSystem.Id)
            {
                return BadRequest();
            }

            _context.Entry(algebricEquationSystem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlgebricEquationSystemExists(id))
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

        // POST: api/AlgebricEquationSystems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AlgebricEquationSystem>> PostAlgebricEquationSystem(AlgebricEquationSystem algebricEquationSystem)
        {
            _context.Equations.Add(algebricEquationSystem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAlgebricEquationSystem", new { id = algebricEquationSystem.Id }, algebricEquationSystem);
        }

        // DELETE: api/AlgebricEquationSystems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlgebricEquationSystem(Guid id)
        {
            var algebricEquationSystem = await _context.Equations.FindAsync(id);
            if (algebricEquationSystem == null)
            {
                return NotFound();
            }

            _context.Equations.Remove(algebricEquationSystem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AlgebricEquationSystemExists(Guid id)
        {
            return _context.Equations.Any(e => e.Id == id);
        }
    }
}

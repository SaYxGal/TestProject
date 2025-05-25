using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Models;

namespace TestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicPerformancesController : ControllerBase
    {
        private readonly DataContext _context;

        public AcademicPerformancesController(DataContext context)
        {
            _context = context;
        }

        // GET: api/AcademicPerformances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcademicPerformance>>> GetAcademicPerformances()
        {
            return await _context.AcademicPerformances.ToListAsync();
        }

        // GET: api/AcademicPerformances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AcademicPerformance>> GetAcademicPerformance(int id)
        {
            var academicPerformance = await _context.AcademicPerformances.FindAsync(id);

            if (academicPerformance == null)
            {
                return NotFound();
            }

            return academicPerformance;
        }

        // PUT: api/AcademicPerformances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAcademicPerformance(int id, AcademicPerformance academicPerformance)
        {
            if (id != academicPerformance.Id)
            {
                return BadRequest();
            }

            _context.Entry(academicPerformance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AcademicPerformanceExists(id))
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

        // POST: api/AcademicPerformances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AcademicPerformance>> PostAcademicPerformance(AcademicPerformance academicPerformance)
        {
            _context.AcademicPerformances.Add(academicPerformance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAcademicPerformance", new { id = academicPerformance.Id }, academicPerformance);
        }

        // DELETE: api/AcademicPerformances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAcademicPerformance(int id)
        {
            var academicPerformance = await _context.AcademicPerformances.FindAsync(id);
            if (academicPerformance == null)
            {
                return NotFound();
            }

            _context.AcademicPerformances.Remove(academicPerformance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AcademicPerformanceExists(int id)
        {
            return _context.AcademicPerformances.Any(e => e.Id == id);
        }
    }
}

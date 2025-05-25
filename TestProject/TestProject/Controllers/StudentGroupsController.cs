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
    public class StudentGroupsController : ControllerBase
    {
        private readonly DataContext _context;

        public StudentGroupsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/StudentGroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentGroup>>> GetStudentGroups()
        {
            return await _context.StudentGroups.ToListAsync();
        }

        // GET: api/StudentGroups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentGroup>> GetStudentGroup(int id)
        {
            var studentGroup = await _context.StudentGroups.FindAsync(id);

            if (studentGroup == null)
            {
                return NotFound();
            }

            return studentGroup;
        }

        // PUT: api/StudentGroups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudentGroup(int id, StudentGroup studentGroup)
        {
            if (id != studentGroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(studentGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentGroupExists(id))
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

        // POST: api/StudentGroups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StudentGroup>> PostStudentGroup(StudentGroup studentGroup)
        {
            _context.StudentGroups.Add(studentGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudentGroup", new { id = studentGroup.Id }, studentGroup);
        }

        // DELETE: api/StudentGroups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentGroup(int id)
        {
            var studentGroup = await _context.StudentGroups.FindAsync(id);
            if (studentGroup == null)
            {
                return NotFound();
            }

            _context.StudentGroups.Remove(studentGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentGroupExists(int id)
        {
            return _context.StudentGroups.Any(e => e.Id == id);
        }
    }
}

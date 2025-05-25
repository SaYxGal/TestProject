using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TestProject.Models;
using TestProject.Models.DTO;
using TestProject.Services;

namespace TestProject.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SubjectsController : ControllerBase
{
    private readonly SubjectService _subjectService;

    public SubjectsController(SubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    // GET: api/Subjects
    [HttpGet]
    public async Task<ActionResult<List<GetSubjectDTO>>> GetSubjects([FromQuery][Required] int from, [FromQuery][Required] int count)
    {
        return await _subjectService.Get(from, count);
    }

    // GET: api/Subjects/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetSubjectDTO>> GetSubject(int id)
    {
        var subject = await _subjectService.GetById(id);

        if (subject == null)
        {
            return NotFound();
        }

        return subject;
    }

    // PUT: api/Subjects/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSubject(int id, UpdateSubjectDTO dto)
    {
        await _subjectService.Update(id, dto);

        return NoContent();
    }

    // POST: api/Subjects
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = UserRole.Teacher)]
    public async Task<ActionResult<Subject>> PostSubject(CreateSubjectDTO dto)
    {
        var subjectId = await _subjectService.Create(dto);

        return CreatedAtAction("GetSubject", new { id = subjectId }, dto);
    }

    // DELETE: api/Subjects/5
    [HttpDelete("{id}")]
    [Authorize(Roles = UserRole.Teacher)]
    public async Task<IActionResult> DeleteSubject(int id)
    {
        await _subjectService.Delete(id);

        return NoContent();
    }
}

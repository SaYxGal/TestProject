using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Models.DTO;

namespace TestProject.Services;

public class SubjectService(DataContext dataContext)
{
    private readonly DataContext _dataContext = dataContext;

    public async Task<List<GetSubjectDTO>> Get(int from, int count)
    {
        return await
            _dataContext.Subjects
                .Skip(from - 1).Take(count)
                .Select(i => new GetSubjectDTO { Name = i.Name })
                .ToListAsync();
    }

    public async Task<GetSubjectDTO> GetById(int id)
    {
        var subject = await Find(id);

        return new GetSubjectDTO { Name = subject.Name };
    }

    public async Task<int> Create(CreateSubjectDTO dto)
    {
        var subject = new Subject()
        {
            Name = dto.Name,
        };

        _dataContext.Subjects.Add(subject);

        await _dataContext.SaveChangesAsync();

        return subject.Id;
    }

    public async Task Update(int id, UpdateSubjectDTO dto)
    {
        var subject = await Find(id);

        subject.Name = dto.Name;

        _dataContext.Update(subject);

        await _dataContext.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var subject = await Find(id);

        _dataContext.Remove(subject);
        await _dataContext.SaveChangesAsync();
    }

    private async Task<Subject> Find(int id)
    {
        return await
            _dataContext.Subjects
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException();
    }
}

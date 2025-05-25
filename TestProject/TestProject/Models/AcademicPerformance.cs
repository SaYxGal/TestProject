namespace TestProject.Models;

public class AcademicPerformance
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public Student Student { get; set; } = null!;

    public int SubjectId { get; set; }

    public Subject Subject { get; set; } = null!;

    public ushort Grade { get; set; }

    public ushort Semester { get; set; }

    public DateTime DateOfRecord { get; set; }
}

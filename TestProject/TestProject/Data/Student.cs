namespace TestProject.Data;

public class Student
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public int GroupId { get; set; }

    public StudentGroup Group { get; set; } = null!;

    public int? AdmissionYear { get; set; }
}

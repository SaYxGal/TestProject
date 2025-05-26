using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace TestProject.Data;

public class DataContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }

    public DbSet<StudentGroup> StudentGroups { get; set; }

    public DbSet<Subject> Subjects { get; set; }

    public DbSet<AcademicPerformance> AcademicPerformances { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Student>()
            .HasOne(i => i.User)
            .WithOne()
            .HasForeignKey<Student>(i => i.UserId)
            .IsRequired();

        builder.Entity<Student>()
            .HasOne(i => i.Group)
            .WithMany()
            .HasForeignKey(i => i.GroupId)
            .IsRequired();

        builder.Entity<AcademicPerformance>()
            .HasOne(i => i.Student)
            .WithMany()
            .HasForeignKey(i => i.StudentId)
            .IsRequired();

        builder.Entity<AcademicPerformance>()
            .HasOne(i => i.Subject)
            .WithMany()
            .HasForeignKey(i => i.SubjectId)
            .IsRequired();

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

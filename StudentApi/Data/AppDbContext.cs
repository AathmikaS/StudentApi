using Microsoft.EntityFrameworkCore;
using StudentApi.Models;
namespace StudentApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
            entity.Property(s => s.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(s => s.Email).IsRequired().HasMaxLength(255);
            entity.Property(s => s.Password).IsRequired().HasMaxLength(255);
            entity.Property(s => s.CreatedDate).IsRequired();

            entity.HasIndex(s => s.Email).IsUnique();
            entity.HasIndex(s => s.PhoneNumber).IsUnique();
        });
    }
}

using CompanyStructureApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyStructureApi.Data;

public class CompanyStructureDbContext : DbContext
{
    public CompanyStructureDbContext(DbContextOptions<CompanyStructureDbContext> options) : base(options) { }

    public DbSet<Company> Companies { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Division> Divisions { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Department> Departments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Company>()
            .HasIndex(c => c.company_code)
            .IsUnique();

        modelBuilder.Entity<Company>()
            .HasOne(c => c.Director)
            .WithMany()
            .HasForeignKey(c => c.director_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.email)
            .IsUnique();

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Company)
            .WithMany(c => c.Employees)
            .HasForeignKey(e => e.company_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Division>()
            .HasIndex(d => new { d.division_code, d.company_id })
            .IsUnique();

        modelBuilder.Entity<Division>()
            .HasOne(d => d.DivisionLeader)
            .WithMany()
            .HasForeignKey(d => d.division_leader_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Project>()
            .HasIndex(p => new { p.project_code, p.division_id })
            .IsUnique();

        modelBuilder.Entity<Project>()
            .HasOne(p => p.ProjectLeader)
            .WithMany()
            .HasForeignKey(p => p.project_leader_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Department>()
            .HasIndex(d => new { d.department_code, d.project_id })
            .IsUnique();

        modelBuilder.Entity<Department>()
            .HasOne(d => d.DepartmentLeader)
            .WithMany()
            .HasForeignKey(d => d.department_leader_id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

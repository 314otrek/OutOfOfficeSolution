using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<EmployeeProject> EmployeeProjects { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Tutaj mo¿esz dodaæ dodatkowe konfiguracje modelu, jeœli s¹ potrzebne
            base.OnModelCreating(modelBuilder);

            /*modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(a => a.ID);
            });

            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasKey(a => a.ID);
                entity.HasOne(a => a.Employee).WithMany(a => a.LeaveRequests).HasForeignKey(a => a.EmployeeId);
            });
            */
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasMany(e => e.LeaveRequests)
                      .WithOne(lr => lr.Employee)
                      .HasForeignKey(lr => lr.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict); // Zmieniono na Restrict, aby unikn¹æ przypadkowego usuniêcia wniosków
            });

            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasKey(lr => lr.ID);
                entity.HasOne(lr => lr.Employee)
                      .WithMany(e => e.LeaveRequests)
                      .HasForeignKey(lr => lr.EmployeeId)
                      .IsRequired();

                entity.HasOne(lr => lr.ApprovalRequest)
                      .WithOne()
                      .HasForeignKey<ApprovalRequest>(ar => ar.LeaveRequestId); // Zak³adaj¹c, ¿e ApprovalRequest ma LeaveRequestId
            });

            modelBuilder.Entity<EmployeeProject>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.HasOne(a => a.Employee).WithMany(a => a.EmployeeProjects).HasForeignKey(a => a.EmployeeId);
                entity.HasOne(a => a.Project).WithMany(a => a.EmployeeProjects).HasForeignKey(a => a.ProjectId);
            });
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(a => a.ID);
                entity.HasOne(a => a.ProjectManager).WithMany(a => a.Projects).HasForeignKey(a => a.ProjectManagerId);
            });
            modelBuilder.Entity<ApprovalRequest>(entity =>
            {
                entity.HasKey(a => a.ID);
                entity.HasOne(a => a.Approver).WithMany(a => a.AprovalRequest).HasForeignKey(a => a.ApproverId);
                entity.HasOne(a => a.LeaveRequest).WithOne(a => a.ApprovalRequest);
            });
        }
    }
}

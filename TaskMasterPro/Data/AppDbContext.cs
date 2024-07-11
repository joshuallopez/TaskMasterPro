// Developed by Josue Lopez Lozano
// Last Updated March 30th, 2024
using Microsoft.EntityFrameworkCore;
using TaskMasterPro.Models;
using Task = TaskMasterPro.Models.Task;

namespace TaskMasterPro.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the one-to-many relationship between User and Task
            modelBuilder.Entity<Task>()
                .HasOne(t => t.User) // Each Task has one User
                .WithMany(u => u.Tasks) // Each User has many Tasks
                .HasForeignKey(t => t.UserId) // The foreign key in the Task table
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            // Configure the one-to-many relationship between User and Project
            modelBuilder.Entity<Project>()
                .HasOne(p => p.User) // Each Project has one User
                .WithMany(u => u.Projects) // Each User has many Projects
                .HasForeignKey(p => p.UserId) // The foreign key in the Project table
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            // If there are foreign key relationships from Tasks to Projects that also cascade, consider setting them to Restrict as well
            modelBuilder.Entity<Task>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Restrict); // Assuming you want to restrict cascading delete here too
        }
    }
}

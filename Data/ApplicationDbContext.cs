using Microsoft.EntityFrameworkCore;
using Onboarding.Models;
using System.Drawing;

namespace Onboarding.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<TaskUser> UserTasks { get; set; }
        public DbSet<CourseUser> CourseUsers { get; set; } //new
        //public DbSet<CourseTask> CourseTasks { get; set; } //new
        public DbSet<Link> Links { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Announcement> Announcements { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            
            if (modelBuilder == null)
                throw new ArgumentNullException("modelBuilder");

            // for the other conventions, we do a metadata model loop
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // equivalent of modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
                entityType.SetTableName(entityType.DisplayName());

                // equivalent of modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
                entityType.GetForeignKeys()
                    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                    .ToList()
                    .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Cascade);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}

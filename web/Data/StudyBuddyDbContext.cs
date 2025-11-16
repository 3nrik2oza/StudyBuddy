using Microsoft.EntityFrameworkCore;
using web.Models.Entities; 

namespace web.Data
{
    public class StudyBuddyDbContext : DbContext
    {
        public StudyBuddyDbContext(DbContextOptions<StudyBuddyDbContext> options)
            : base(options)
        {
        }

        public DbSet<Material> Materials { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudyPost> StudyPosts { get; set; }
        public DbSet<Tutor> Tutors { get; set; } 
        public DbSet<TutorSubject> TutorSubjects { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<ForumThread> ForumThreads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

             modelBuilder.Entity<TutorSubject>()
            .HasKey(ts => new { ts.UserId, ts.SubjectId });

            modelBuilder.Entity<TutorSubject>()
                .HasOne(ts => ts.Tutor)
                .WithMany(t => t.TutorSubjects)
                .HasForeignKey(ts => ts.UserId);

            modelBuilder.Entity<TutorSubject>()
                .HasOne(ts => ts.Subject)
                .WithMany() 
                .HasForeignKey(ts => ts.SubjectId);
        }
    }
}

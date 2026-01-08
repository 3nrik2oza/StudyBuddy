using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using web.Models;
using web.Models.Entities;

namespace web.Data
{
    public class StudyBuddyDbContext : IdentityDbContext<ApplicationUser>
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
        public DbSet<TutorRequestMessage> TutorRequestMessages { get; set; }

        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<ForumThread> ForumThreads { get; set; }
        public DbSet<ForumReply> ForumReplies { get; set; }
        public DbSet<StudyPostParticipant> StudyPostParticipants { get; set; }
        public DbSet<TutorRequest> TutorRequests { get; set; }
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

            modelBuilder.Entity<ForumReply>()
                .HasOne(r => r.ForumThread)
                .WithMany(t => t.Replies)
                .HasForeignKey(r => r.ForumThreadId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<StudyPostParticipant>()
                .HasIndex(x => new { x.StudyPostId, x.UserId })
                .IsUnique();

            modelBuilder.Entity<StudyPostParticipant>()
                .HasOne(x => x.StudyPost)
                .WithMany()
                .HasForeignKey(x => x.StudyPostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudyPostParticipant>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TutorRequest>()
                .HasOne(x => x.StudentUser)
                .WithMany()
                .HasForeignKey(x => x.StudentUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TutorRequest>()
                .HasOne(x => x.TutorUser)
                .WithMany()
                .HasForeignKey(x => x.TutorUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TutorRequest>()
                .HasOne(x => x.Subject)
                .WithMany()
                .HasForeignKey(x => x.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TutorRequestMessage>()
                .HasOne(m => m.TutorRequest)
                .WithMany(r => r.Messages)
                .HasForeignKey(m => m.TutorRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TutorRequestMessage>()
                .HasOne(m => m.SenderUser)
                .WithMany()
                .HasForeignKey(m => m.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}

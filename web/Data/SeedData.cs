using web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace web.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StudyBuddyDbContext>();

        context.Database.Migrate();

        if (!context.Faculties.Any())
        {
            context.Faculties.Add(new Faculty
            {
                Id = 1,
                Name = "FRI",
                Slug = "fri"
            });
            context.SaveChanges();
        }

        if (!context.Subjects.Any())
        {
            context.Subjects.AddRange(
                new Subject { Id = 1, Name = "Information Systems", Code = "IS",   FacultyId = 1 },
                new Subject { Id = 2, Name = "APS1",                Code = "APS1", FacultyId = 1 }
            );
            context.SaveChanges();
        }

        if (!context.Tutors.Any())
        {
            context.Tutors.AddRange(
                new Tutor
                {
                    Id         = "tutor1",
                    Name       = "Ana Novak",
                    FacultyId  = 1,
                    HelpPoints = 8
                },
                new Tutor
                {
                    Id         = "tutor2",
                    Name       = "Marko Kovač",
                    FacultyId  = 1,
                    HelpPoints = 5
                },
                new Tutor
                {
                    Id         = "tutor3",
                    Name       = "Sara Horvat",
                    FacultyId  = 1,
                    HelpPoints = 12
                }
            );
            context.SaveChanges();
        }

        if (!context.TutorSubjects.Any())
        {
            context.TutorSubjects.AddRange(
                new TutorSubject { UserId = "tutor1", SubjectId = 1 },
                new TutorSubject { UserId = "tutor1", SubjectId = 2 },
                new TutorSubject { UserId = "tutor2", SubjectId = 2 },
                new TutorSubject { UserId = "tutor3", SubjectId = 1 }
            );
            context.SaveChanges();
        }

        if (!context.Materials.Any())
        {
            context.Materials.AddRange(
                new Material
                {
                    Id           = 1,
                    Title        = "IS – midterm notes",
                    Description  = "Summarized notes from lectures and labs for the first IS midterm.",
                    Type         = "link",
                    Url          = "https://example.com/is-skripta",
                    SubjectId    = 1,      // Information Systems
                    FacultyId    = 1,      // FRI
                    AuthorUserId = "demo1",
                    CreatedAt    = DateTime.UtcNow.AddDays(-3)
                },
                new Material
                {
                    Id           = 2,
                    Title        = "APS1 – exercises with solutions",
                    Description  = "A collection of exercises on recursion and sorting.",
                    Type         = "link",
                    Url          = "https://example.com/aps1-zadaci",
                    SubjectId    = 2,      // APS1
                    FacultyId    = 1,
                    AuthorUserId = "demo2",
                    CreatedAt    = DateTime.UtcNow.AddDays(-1)
                },
                new Material
                {
                    Id           = 3,
                    Title        = "IS – UML diagram example",
                    Description  = "Example UML diagram for a seminar project.",
                    Type         = "link",
                    Url          = "https://example.com/is-uml",
                    SubjectId    = 1,
                    FacultyId    = 1,
                    AuthorUserId = "demo3",
                    CreatedAt    = DateTime.UtcNow.AddHours(-5)
                }
            );
            context.SaveChanges();
        }

        if (!context.StudyPosts.Any())
        {
            var tomorrowAt17Utc = DateTime.UtcNow.Date.AddDays(1).AddHours(17);
            context.StudyPosts.Add(
                new StudyPost
                {
                    Id           = 1,
                    Title        = "IS – midterm revision",
                    SubjectId    = 1,
                    StartAt      = tomorrowAt17Utc,
                    Location     = "FRI P22",
                    IsOnline     = false,
                    FacultyId    = 1,
                    AuthorUserId = "demo",
                    CreatedAt    = DateTime.UtcNow.AddDays(-1)
                }
            );
            context.SaveChanges();
        }

        if (!context.ForumThreads.Any())
        {
            context.ForumThreads.AddRange(
                new ForumThread
                {
                    Id           = 1,
                    Title        = "Looking for APS1 study group",
                    Content      = "Hi, I am looking for 2–3 people to study APS1 together before the midterm.",
                    Category     = "Study group",
                    SubjectId    = 2, // APS1
                    FacultyId    = 1,
                    AuthorName   = "Ana",
                    CreatedAt    = DateTime.UtcNow.AddDays(-2),
                    RepliesCount = 5
                },
                new ForumThread
                {
                    Id           = 2,
                    Title        = "Sharing IS notes from last year",
                    Content      = "I uploaded my IS notes from last year. They cover all lectures and most of the lab exercises.",
                    Category     = "Materials",
                    SubjectId    = 1, // IS
                    FacultyId    = 1,
                    AuthorName   = "Marko",
                    CreatedAt    = DateTime.UtcNow.AddDays(-1),
                    RepliesCount = 10
                },
                new ForumThread
                {
                    Id           = 3,
                    Title        = "Help with recursion in APS1",
                    Content      = "Can someone explain recursion with a simple example? I am stuck on the homework exercises.",
                    Category     = "Help",
                    SubjectId    = 2,
                    FacultyId    = 1,
                    AuthorName   = "Sara",
                    CreatedAt    = DateTime.UtcNow.AddHours(-8),
                    RepliesCount = 35
                },
                new ForumThread
                {
                    Id           = 4,
                    Title        = "Project team for IS",
                    Content      = "I am looking for 2 more people to form a project team for StudyBuddy or a similar IS project.",
                    Category     = "Study group",
                    SubjectId    = 1,
                    FacultyId    = 1,
                    AuthorName   = "Lejla",
                    CreatedAt    = DateTime.UtcNow.AddHours(-3),
                    RepliesCount = 3
                }
            );
            context.SaveChanges();
        }
    }
}

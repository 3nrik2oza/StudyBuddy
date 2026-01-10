using Microsoft.EntityFrameworkCore;
using web.Models.Entities;

namespace web.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StudyBuddyDbContext>();

        context.Database.Migrate();

        // -------------------------
        // Faculties (FRI, MF, FKKT)
        // -------------------------
        EnsureFaculty(context, id: 1, name: "FRI",  slug: "fri");
        EnsureFaculty(context, id: 2, name: "MF",   slug: "mf");
        EnsureFaculty(context, id: 3, name: "FKKT", slug: "fkkt");

        // -------------------------
        // Subjects
        // -------------------------
        // FRI (existing)
        EnsureSubject(context, id: 1, name: "Information Systems", code: "IS",   facultySlug: "fri");
        EnsureSubject(context, id: 2, name: "APS1",                code: "APS1", facultySlug: "fri");

        // MF (new)
        EnsureSubject(context, id: 101, name: "Anatomy",    code: "ANATOMY",   facultySlug: "mf");
        EnsureSubject(context, id: 102, name: "Physiology", code: "PHYSIOLOGY", facultySlug: "mf");

        // FKKT (new)
        EnsureSubject(context, id: 201, name: "Physics",             code: "PHYSICS",  facultySlug: "fkkt");
        EnsureSubject(context, id: 202, name: "Inorganic Chemistry", code: "INORG",    facultySlug: "fkkt");

        context.SaveChanges();

        // -------------------------
        // Tutors (keep your old demo, but make it safe)
        // -------------------------
        if (!context.Tutors.Any())
        {
            context.Tutors.AddRange(
                new Tutor { Id = "tutor1", Name = "Ana Novak",   FacultyId = 1, HelpPoints = 8 },
                new Tutor { Id = "tutor2", Name = "Marko Kovač", FacultyId = 1, HelpPoints = 5 },
                new Tutor { Id = "tutor3", Name = "Sara Horvat", FacultyId = 1, HelpPoints = 12 }
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

        // -------------------------
        // Materials (keep your old demo, but make it safe)
        // -------------------------
        if (!context.Materials.Any())
        {
            context.Materials.AddRange(
                new Material
                {
                    Id = 1,
                    Title = "IS – midterm notes",
                    Description = "Summarized notes from lectures and labs for the first IS midterm.",
                    Type = "link",
                    Url = "https://example.com/is-skripta",
                    SubjectId = 1,
                    FacultyId = 1,
                    AuthorUserId = "demo1",
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new Material
                {
                    Id = 2,
                    Title = "APS1 – exercises with solutions",
                    Description = "A collection of exercises on recursion and sorting.",
                    Type = "link",
                    Url = "https://example.com/aps1-zadaci",
                    SubjectId = 2,
                    FacultyId = 1,
                    AuthorUserId = "demo2",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new Material
                {
                    Id = 3,
                    Title = "IS – UML diagram example",
                    Description = "Example UML diagram for a seminar project.",
                    Type = "link",
                    Url = "https://example.com/is-uml",
                    SubjectId = 1,
                    FacultyId = 1,
                    AuthorUserId = "demo3",
                    CreatedAt = DateTime.UtcNow.AddHours(-5)
                }
            );
            context.SaveChanges();
        }

        // -------------------------
        // StudyPosts (keep your old demo, but make it safe)
        // -------------------------
        if (!context.StudyPosts.Any())
        {
            var tomorrowAt17Utc = DateTime.UtcNow.Date.AddDays(1).AddHours(17);
            context.StudyPosts.Add(
                new StudyPost
                {
                    Id = 1,
                    Title = "IS – midterm revision",
                    SubjectId = 1,
                    StartAt = tomorrowAt17Utc,
                    Location = "FRI P22",
                    IsOnline = false,
                    FacultyId = 1,
                    AuthorUserId = "demo",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            );
            context.SaveChanges();
        }
    }

    private static void EnsureFaculty(StudyBuddyDbContext context, int id, string name, string slug)
    {
        var exists = context.Faculties.Any(f => f.Slug == slug);
        if (exists) return;

        context.Faculties.Add(new Faculty
        {
            Id = id,
            Name = name,
            Slug = slug
        });
    }

    private static void EnsureSubject(StudyBuddyDbContext context, int id, string name, string code, string facultySlug)
    {
        var faculty = context.Faculties.FirstOrDefault(f => f.Slug == facultySlug);
        if (faculty == null) return;

        var exists = context.Subjects.Any(s => s.Code == code);
        if (exists) return;

        context.Subjects.Add(new Subject
        {
            Id = id,
            Name = name,
            Code = code,
            FacultyId = faculty.Id
        });
    }
}

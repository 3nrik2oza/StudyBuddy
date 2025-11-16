using web.Models.Entities;

namespace web.Services;

public class FakeTutor
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int FacultyId { get; set; }
    public int HelpPoints { get; set; }
}

public static class InMemoryData
{
    public static List<Faculty> Faculties { get; } = new()
    {
        new Faculty { Id = 1, Name = "FRI", Slug = "fri" }
    };

    public static List<Subject> Subjects { get; } = new()
    {
        new Subject { Id = 1, Name = "Informacijski sistemi", Code = "IS",   FacultyId = 1 },
        new Subject { Id = 2, Name = "APS1",                  Code = "APS1", FacultyId = 1 }
    };

    public static List<Material> Materials { get; } = new()
    {
        new Material
        {
            Id          = 1,
            Title       = "IS – skripta za kolokvij",
            Description = "Sažeti zapisci sa predavanja i vajbi za prvi kolokvij iz IS-a.",
            Type        = "link",
            Url         = "https://example.com/is-skripta",
            SubjectId   = 1,      // Informacijski sistemi
            FacultyId   = 1,      // FRI
            AuthorUserId= "demo1",
            CreatedAt   = DateTime.UtcNow.AddDays(-3)
        },
        new Material
        {
            Id          = 2,
            Title       = "APS1 – zadaci sa rješenjima",
            Description = "Zbirka zadataka iz rekurzije i sortiranja.",
            Type        = "link",
            Url         = "https://example.com/aps1-zadaci",
            SubjectId   = 2,      // APS1
            FacultyId   = 1,
            AuthorUserId= "demo2",
            CreatedAt   = DateTime.UtcNow.AddDays(-1)
        },
        new Material
        {
            Id          = 3,
            Title       = "IS – UML dijagrami primjer",
            Description = "Primjer UML dijagrama za seminarski projekat.",
            Type        = "link",
            Url         = "https://example.com/is-uml",
            SubjectId   = 1,
            FacultyId   = 1,
            AuthorUserId= "demo3",
            CreatedAt   = DateTime.UtcNow.AddHours(-5)
        }
    };

    public static List<StudyPost> StudyPosts { get; } = new()
    {
        new StudyPost
        {
            Id          = 1,
            Title       = "IS – ponavljanje za kolokvij",
            SubjectId   = 1,
            StartAt     = DateTime.Today.AddDays(1).AddHours(17),
            Location    = "FRI P22",
            IsOnline    = false,
            FacultyId   = 1,
            AuthorUserId= "demo",
            CreatedAt   = DateTime.UtcNow.AddDays(-1)
        }
    };

    public static List<FakeTutor> Tutors { get; } = new()
    {
        new FakeTutor
        {
            Id         = "tutor1",
            Name       = "Ana Novak",
            FacultyId  = 1,
            HelpPoints = 8
        },
        new FakeTutor
        {
            Id         = "tutor2",
            Name       = "Marko Kovač",
            FacultyId  = 1,
            HelpPoints = 5
        },
        new FakeTutor
        {
            Id         = "tutor3",
            Name       = "Sara Horvat",
            FacultyId  = 1,
            HelpPoints = 12
        }
    };

    public static List<TutorSubject> TutorSubjects { get; } = new()
    {
        new TutorSubject { UserId = "tutor1", SubjectId = 1 },
        new TutorSubject { UserId = "tutor1", SubjectId = 2 },
        new TutorSubject { UserId = "tutor2", SubjectId = 2 },
        new TutorSubject { UserId = "tutor3", SubjectId = 1 }
    };

    public static List<News> NewsItems { get; } = new()
    {
        new News
        {
            Id        = 1,
            Category  = "New material",
            Title     = "New Math 1 notes uploaded",
            Content   = "A new set of exercises and concise formula summaries for the Math 1 exam is now available.",
            FacultyId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            LinkText  = "View material",
            LinkUrl   = "/Materials"
        },
        new News
        {
            Id        = 2,
            Category  = "Course update",
            Title     = "New Python mini-projects",
            Content   = "We added new practice projects: a to-do app, a calculator and a quiz application.",
            FacultyId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            LinkText  = "See what’s new",
            LinkUrl   = "/Materials"
        },
        new News
        {
            Id        = 3,
            Category  = "Announcement",
            Title     = "Live Q&A with tutors",
            Content   = "Every Sunday at 19:00 you can join an online Q&A session about programming and mathematics.",
            FacultyId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            LinkText  = "Join the session",
            LinkUrl   = "/StudySessions"
        },
        new News
        {
            Id        = 4,
            Category  = "Featured",
            Title     = "Top 10 materials this month",
            Content   = "Check which resources students are using and recommending the most for studying.",
            FacultyId = 1,
            CreatedAt = DateTime.UtcNow.AddHours(-6),
            LinkText  = "Open list",
            LinkUrl   = "/Materials"
        }
    };
}

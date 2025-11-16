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
        new Subject { Id = 1, Name = "Information Systems", Code = "IS",   FacultyId = 1 },
        new Subject { Id = 2, Name = "APS1",                Code = "APS1", FacultyId = 1 }
    };

    public static List<Material> Materials { get; } = new()
    {
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
    };

    public static List<StudyPost> StudyPosts { get; } = new()
    {
        new StudyPost
        {
            Id           = 1,
            Title        = "IS – midterm revision",
            SubjectId    = 1,
            StartAt      = DateTime.Today.AddDays(1).AddHours(17),
            Location     = "FRI P22",
            IsOnline     = false,
            FacultyId    = 1,
            AuthorUserId = "demo",
            CreatedAt    = DateTime.UtcNow.AddDays(-1)
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

    public static List<ForumThread> ForumThreads { get; } = new()
    {
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
    };
}

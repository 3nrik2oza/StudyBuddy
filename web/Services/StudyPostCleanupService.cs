using Microsoft.EntityFrameworkCore;
using web.Data;

namespace web.Services;

public class StudyPostCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public StudyPostCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Cleanup(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task Cleanup(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StudyBuddyDbContext>();

        var now = DateTime.UtcNow;

        var expiredIds = await db.StudyPosts
            .Where(p => p.StartAt < now)
            .Select(p => p.Id)
            .ToListAsync(ct);

        if (expiredIds.Count == 0)
            return;

        var participants = await db.StudyPostParticipants
            .Where(x => expiredIds.Contains(x.StudyPostId))
            .ToListAsync(ct);

        db.StudyPostParticipants.RemoveRange(participants);

        var posts = await db.StudyPosts
            .Where(p => expiredIds.Contains(p.Id))
            .ToListAsync(ct);

        db.StudyPosts.RemoveRange(posts);

        await db.SaveChangesAsync(ct);
    }
}

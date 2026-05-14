using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Models;
using web.Models.Entities;

namespace web.Hubs;

[Authorize]
public class TutorRequestChatHub : Hub
{
    private readonly StudyBuddyDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TutorRequestChatHub(
        StudyBuddyDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task JoinRequestGroup(int requestId)
    {
        var userId = Context.UserIdentifier;

        if (string.IsNullOrWhiteSpace(userId))
            return;

        var allowed = await _context.TutorRequests.AnyAsync(r =>
            r.Id == requestId &&
            (r.StudentUserId == userId || r.TutorUserId == userId));

        if (!allowed)
            return;

        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(requestId));
    }

    public async Task SendMessage(int requestId, string body)
    {
        var userId = Context.UserIdentifier;

        if (string.IsNullOrWhiteSpace(userId))
            throw new HubException("Unauthorized.");

        body = (body ?? "").Trim();

        if (body.Length == 0)
            throw new HubException("Message cannot be empty.");

        if (body.Length > 2000)
            body = body[..2000];

        var request = await _context.TutorRequests.FirstOrDefaultAsync(r =>
            r.Id == requestId &&
            (r.StudentUserId == userId || r.TutorUserId == userId));

        if (request == null)
            throw new HubException("Request not found.");

        var message = new TutorRequestMessage
        {
            TutorRequestId = requestId,
            SenderUserId = userId,
            Body = body,
            SentAt = DateTime.UtcNow
        };

        _context.TutorRequestMessages.Add(message);

        var receiverId = request.StudentUserId == userId
            ? request.TutorUserId
            : request.StudentUserId;

        var sender = await _userManager.FindByIdAsync(userId);

        var senderName = !string.IsNullOrWhiteSpace(sender?.Name)
            ? sender.Name
            : sender?.Email ?? "User";

        var notification = new Notification
        {
            UserId = receiverId,
            Title = $"New message from {senderName}",
            Message = body.Length > 120 ? body[..120] + "..." : body,
            Link = $"/Tutors/RequestDetails/{requestId}",
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();

        await Clients.Group(GetGroupName(requestId)).SendAsync("ReceiveMessage", new
        {
            senderUserId = userId,
            senderName,
            body,
            sentAt = message.SentAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm")
        });

        await Clients.User(receiverId).SendAsync("ReceiveNotification", new
        {
            title = notification.Title,
            message = notification.Message,
            link = notification.Link
        });
    }

    private static string GetGroupName(int requestId)
    {
        return $"tutor-request-{requestId}";
    }
}
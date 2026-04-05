using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Web.Infrastructure;

namespace VolunteerHub.Web.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyNotifications(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _notificationService.GetMyNotificationsAsync(userId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.Error });
    }

    [HttpGet("my/unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _notificationService.GetMyUnreadCountAsync(userId, cancellationToken);
        return result.IsSuccess ? Ok(new { Count = result.Value }) : BadRequest(new { result.Error });
    }

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _notificationService.MarkAsReadAsync(userId, id, cancellationToken);
        return result.IsSuccess ? Ok(new { Message = "Notification marked as read." }) : NotFound(new { result.Error });
    }
}

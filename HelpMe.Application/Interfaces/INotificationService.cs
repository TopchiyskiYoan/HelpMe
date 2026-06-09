using HelpMe.Application.DTOs;
using HelpMe.Domain.Entities;

namespace HelpMe.Application.Interfaces;

public interface INotificationService
{
    Task CreateAsync(string userId, NotificationType type, string title, string message);
    Task<List<NotificationDto>> GetUserNotificationsAsync(string userId);
    Task<bool> MarkAsReadAsync(int id, string userId);
    Task MarkAllAsReadAsync(string userId);
    Task<int> GetUnreadCountAsync(string userId);
}
